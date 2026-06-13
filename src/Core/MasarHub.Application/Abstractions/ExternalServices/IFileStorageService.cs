using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;

namespace MasarHub.Application.Abstractions.ExternalServices
{
    public interface IFileStorageService : IScopedService
    {
        Task<Result<StoredFile>> UploadAsync(FileResource file, FileType fileType, string folder, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(string fileKey, FileType fileType, CancellationToken cancellationToken = default);
        string GetUrl(string fileKey, FileType fileType);
        UploadSignatureParams GenerateUploadSignature(FileType fileType, string folder);
        Task<Result<StoredFile>> GetVideoMetadataAsync(string fileKey, CancellationToken cancellationToken = default);
    }
}
