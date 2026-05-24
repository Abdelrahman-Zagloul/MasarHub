namespace MasarHub.Application.Features.Categories.Commands.CreateCategory
{
    public sealed record CreateCategoryResponse
    (
        Guid Id,
        string Name,
        string Slug,
        int Level,
        int DisplayOrder,
        Guid? ParentCategoryId
    );
}
