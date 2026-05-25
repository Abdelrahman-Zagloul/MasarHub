using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Categories.Commands.ReorderCategories
{
    public sealed record ReorderCategoriesCommand
    (
        Guid? ParentCategoryId,
        IReadOnlyList<Guid> OrderedCategoryIds
    ) : IRequest<Result>;
}
