using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Features.Categories.Queries.GetCategoryById;
using MasarHub.Domain.Modules.Categories;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ICategoryQuery : IScopedService
    {
        Task<bool> CategoryExistsAsync(Guid id, CancellationToken ct = default);
        Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<CategoryWithChildrenResponse?> GetWithChildrenByIdAsync(Guid id, CancellationToken ct = default);
        Task<(int DisplayOrder, bool SlugExists)> GetCreationDataAsync(string slug, Guid? parentCategoryId, CancellationToken ct = default);
        Task<(bool hasChildren, bool hasCourses)> CanDeleteAsync(Guid id, CancellationToken ct = default);
        Task<(int TotalCount, List<CategoryResponse> Categories)> GetAllAsync(int pageNumber, int pageSize, string? categoryName, int? level, CancellationToken ct = default);
    }
}
