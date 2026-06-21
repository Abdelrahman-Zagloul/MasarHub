using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Carts.Commands.ClearCart
{
    public sealed record ClearCartCommand(Guid UserId) : IRequest<Result>;
}