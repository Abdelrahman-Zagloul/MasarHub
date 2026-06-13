using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using ApplicationError = MasarHub.Application.Common.Results.Errors.Error;

namespace MasarHub.Infrastructure.ExternalServices
{
    public sealed class CloudinaryFileStorageService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _cloudinarySettings;
        private readonly FileStorageSettings _fileStorageSettings;
        private readonly ILogger<CloudinaryFileStorageService> _logger;
        private const string StorageType = "authenticated";
        public CloudinaryFileStorageService(Cloudinary cloudinary, IOptions<CloudinarySettings> cloudinaryOptions, IOptions<FileStorageSettings> fileOptions, ILogger<CloudinaryFileStorageService> logger)
        {
            _cloudinary = cloudinary;
            _cloudinarySettings = cloudinaryOptions.Value;
            _fileStorageSettings = fileOptions.Value;
            _logger = logger;
        }

        public async Task<Result<StoredFile>> UploadAsync(FileResource file, FileType fileType, string folder, CancellationToken cancellationToken = default)
        {
            var validationResult = ValidateFileUpload(file, fileType);
            if (validationResult.IsFailure)
                return validationResult.Errors[0];

            try
            {
                var result = await UploadToCloudinaryAsync(file, fileType, folder, cancellationToken);
                if (result.Error != null)
                {
                    _logger.LogError("Failed to upload file '{FileName}' to Cloudinary. Error: {ErrorMessage}", file.FileName, result.Error.Message);
                    return ApplicationError.Failure("storage.file_upload_failed");
                }

                _logger.LogInformation("File '{FileName}' uploaded successfully to Cloudinary with PublicId '{PublicId}'.", file.FileName, result.PublicId);

                double duration = 0;
                if (result is VideoUploadResult videoResult)
                    duration = videoResult.Duration;
                return new StoredFile(result.PublicId, file.FileName, file.ContentType, file.FileSizeInByte, GetUrl(result.PublicId, fileType), duration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while uploading file '{FileName}' to Cloudinary.", file.FileName);
                return ApplicationError.Failure("storage.file_upload_failed");
            }
        }
        public async Task<Result> DeleteAsync(string fileKey, FileType fileType, CancellationToken cancellationToken = default)
        {
            var deletionParams = new DeletionParams(fileKey)
            {
                Type = StorageType,
                ResourceType = MapToResourceType(fileType)
            };

            var result = await _cloudinary.DestroyAsync(deletionParams);
            if (result.Error != null)
            {
                _logger.LogError("Failed to delete file with key '{FileKey}' from Cloudinary. Error: {ErrorMessage}", fileKey, result.Error.Message);
                return ApplicationError.NotFound("storage.file_not_found");
            }

            _logger.LogInformation("File with key '{FileKey}' deleted successfully from Cloudinary.", fileKey);
            return Result.Success();
        }
        public string GetUrl(string fileKey, FileType fileType)
        {
            return MapToUrlBuilder(fileType)
            .Signed(true)
            .Type(StorageType)
            .BuildUrl(fileKey);
        }
        public UploadSignatureParams GenerateUploadSignature(FileType fileType, string folder)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var parameters = new SortedDictionary<string, object>
            {
                ["timestamp"] = timestamp,
                ["type"] = StorageType,
                ["folder"] = folder
            };

            var signature = _cloudinary.Api.SignParameters(parameters);

            return new UploadSignatureParams(
                _cloudinarySettings.CloudName,
                _cloudinarySettings.APIKey,
                signature,
                timestamp,
                folder,
                MapToResourceType(fileType).ToString(),
                StorageType
            );
        }
        public async Task<Result<StoredFile>> GetVideoMetadataAsync(string fileKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var getResourceParams = new GetResourceParams(fileKey)
                {
                    ResourceType = ResourceType.Video,
                    Type = StorageType,
                    ImageMetadata = true,
                };

                var resourceResult = await _cloudinary.GetResourceAsync(getResourceParams, cancellationToken);

                if (resourceResult.Error != null)
                {
                    _logger.LogError("Cloudinary GetResource failed for key '{FileKey}'. Error: {ErrorMessage}", fileKey, resourceResult.Error.Message);
                    return ApplicationError.NotFound("storage.file_not_found");
                }

                var contentType = $"video/{resourceResult.Format ?? "mp4"}";
                var duration = resourceResult.JsonObj?["duration"]?.Value<double>() ?? 0;

                return new StoredFile
                (
                    fileKey,
                    resourceResult.OriginalFilename ?? fileKey,
                    contentType,
                    resourceResult.Bytes,
                    GetUrl(fileKey, FileType.Video),
                    duration
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching metadata for video key '{FileKey}'.", fileKey);
                return ApplicationError.Failure("storage.connection_failed");
            }
        }
        private Result ValidateFileUpload(FileResource file, FileType fileType)
        {
            if (file is null || file.FileSizeInByte <= 0)
                return ApplicationError.Failure("validation.invalid_file");

            if (string.IsNullOrWhiteSpace(file.FileName))
                return ApplicationError.BadRequest("validation.invalid_file_name");

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            var (maxSizeInMb, allowedExtensions) = fileType switch
            {
                FileType.Image => (_fileStorageSettings.MaxImageSizeInMB, _fileStorageSettings.AllowedImageExtensions),
                FileType.Video => (_fileStorageSettings.MaxVideoSizeInMB, _fileStorageSettings.AllowedVideoExtensions),
                FileType.Document => (_fileStorageSettings.MaxDocumentSizeInMB, _fileStorageSettings.AllowedDocumentExtensions),
                FileType.Attachment => (_fileStorageSettings.MaxAttachmentSizeInMB, _fileStorageSettings.AllowedAttachmentExtensions),
                _ => throw new NotSupportedException()
            };

            if (!allowedExtensions.Any(x => x.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                return ApplicationError.BadRequest("validation.invalid_file_extension", metadata: new()
                {
                    ["CurrentExtension"] = extension ?? "none",
                    ["SupportedExtensions"] = string.Join(", ", allowedExtensions)
                });

            if (file.FileSizeInByte > maxSizeInMb * 1024L * 1024L)
                return ApplicationError.BadRequest("validation.file_size_exceeded", metadata: new()
                {
                    ["FileSizeInMB"] = Math.Round(file.FileSizeInByte / 1024d / 1024d, 2),
                    ["MaxSizeInMB"] = maxSizeInMb
                });

            return Result.Success();
        }
        private async Task<UploadResult> UploadToCloudinaryAsync(FileResource file, FileType fileType, string? folder, CancellationToken cancellationToken)
        {
            var fileDescription = new FileDescription(file.FileName, file.Content);
            UploadResult result = fileType switch
            {
                FileType.Image => await _cloudinary.UploadAsync(new ImageUploadParams
                {
                    File = fileDescription,
                    Folder = folder,
                    UseFilename = true,
                    UniqueFilename = true,
                    Type = StorageType
                }, cancellationToken),

                FileType.Video => await _cloudinary.UploadAsync(new VideoUploadParams
                {
                    File = fileDescription,
                    Folder = folder,
                    UseFilename = true,
                    UniqueFilename = true,
                    Type = StorageType
                }, cancellationToken),

                FileType.Document or FileType.Attachment => await _cloudinary.UploadAsync(new RawUploadParams
                {
                    File = fileDescription,
                    Folder = folder,
                    UseFilename = true,
                    UniqueFilename = true,
                    Type = StorageType
                }, cancellationToken: cancellationToken),

                _ => throw new NotSupportedException($"File type '{fileType}' is not supported.")
            };


            return result;
        }
        private static ResourceType MapToResourceType(FileType fileType) => fileType switch
        {
            FileType.Image => ResourceType.Image,
            FileType.Video => ResourceType.Video,
            FileType.Document or FileType.Attachment => ResourceType.Raw,
            _ => throw new NotSupportedException($"FileType '{fileType}' is not supported.")
        };
        private Url MapToUrlBuilder(FileType fileType) => fileType switch
        {
            FileType.Image => _cloudinary.Api.UrlImgUp,
            FileType.Video => _cloudinary.Api.UrlVideoUp,
            FileType.Document or FileType.Attachment => _cloudinary.Api.Url.ResourceType("raw"),
            _ => throw new NotSupportedException($"FileType '{fileType}' is not supported.")
        };


    }
}