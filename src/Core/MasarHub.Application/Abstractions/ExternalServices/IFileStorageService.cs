using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Common.Results;

namespace MasarHub.Application.Abstractions.ExternalServices
{
    public interface IFileStorageService : IScopedService
    {
        Task<Result<StoredFile>> UploadAsync(FileResource file, FileType fileType, string? folder = null, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(string fileKey, FileType fileType, CancellationToken cancellationToken = default);
        string GetUrl(string fileKey, FileType fileType);
    }
}
