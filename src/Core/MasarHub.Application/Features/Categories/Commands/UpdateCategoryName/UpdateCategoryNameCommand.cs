using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Categories.Commands.UpdateCategoryName
{
    public sealed record UpdateCategoryNameCommand(Guid Id, string Name) : IRequest<Result>;
    public sealed record UpdateCategoryNameRequest(string Name);

}
