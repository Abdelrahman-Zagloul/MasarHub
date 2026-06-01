using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Categories.Commands.CreateCategory
{
    public sealed record CreateCategoryCommand
    (
        string Name,
        string? Description,
        Guid? ParentCategoryId
    ) : IRequest<Result<CreateCategoryResponse>>;
}
