namespace MasarHub.Application.Features.Categories.Queries.GetCategoryById
{
    public sealed record CategoryWithChildrenResponse
    (
        CategoryResponse Category,
        IReadOnlyList<CategoryResponse> SubCategories
    );

    public sealed record CategoryResponse
    (
        Guid Id,
        string Name,
        string? Description,
        string Slug,
        int Level,
        int DisplayOrder,
        Guid? ParentCategoryId,
        DateTimeOffset CreatedAt
    );
}
