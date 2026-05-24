using MasarHub.Application.Common.DI;
using MasarHub.Domain.Modules.Categories;

namespace MasarHub.Application.Abstractions.Queries
{
    public interface ICategoryQuery : IScopedService
    {
        Task<bool> CategoryExistsAsync(Guid id, CancellationToken ct = default);
        Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<(int DisplayOrder, bool SlugExists)> GetCreationDataAsync(string slug, Guid? parentCategoryId, CancellationToken ct = default);

    }
}
