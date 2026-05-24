using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Categories.Commands.DeleteCategory
{
    public sealed record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
}
