using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Categories.Commands.UpdateCategory
{
    public sealed record UpdateCategoryCommand(Guid Id, string? Name, Guid? ParentCategoryId) : IRequest<Result>;
    public sealed record UpdateCategoryRequest(string? Name, Guid? ParentCategoryId);

}
