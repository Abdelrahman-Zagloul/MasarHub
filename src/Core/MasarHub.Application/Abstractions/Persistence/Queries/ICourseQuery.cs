using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ICourseQuery : IScopedService
    {
        Task<(bool CategoryExists, int SlugCount)> GetCreationDataAsync(string slug, Guid categoryId, CancellationToken ct = default);
        Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct = default);
    }
}
