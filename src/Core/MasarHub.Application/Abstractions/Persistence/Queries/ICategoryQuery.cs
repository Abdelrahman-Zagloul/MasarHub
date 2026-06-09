using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Categories.Queries.GetCategories;
using MasarHub.Application.Features.Categories.Queries.GetCategoryById;
using MasarHub.Domain.Modules.Categories;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ICategoryQuery : IScopedService
    {
        Task<bool> CategoryExistsAsync(Guid id, CancellationToken ct = default);
        Task<bool> HasChildrenAsync(Guid id, CancellationToken ct = default);
        Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<CategoryWithChildrenResponse?> GetWithChildrenByIdAsync(Guid id, CancellationToken ct = default);
        Task<CategoryCreationData> GetCreationDataAsync(string slug, Guid? parentCategoryId, CancellationToken ct = default);
        Task<CategoryDeletionCheckData> CanDeleteAsync(Guid id, CancellationToken ct = default);
        Task<PagedResult<CategoryResponse>> GetAllAsync(GetCategoriesQuery query, CancellationToken ct = default);
    }

    public sealed record CategoryCreationData(int NextDisplayOrder, bool SlugExists);
    public sealed record CategoryDeletionCheckData(bool HasChildren, bool HasCourses);
}
