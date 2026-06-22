using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Orders.Commands.CancelOrder
{
    public sealed record CancelOrderCommand(Guid UserId, Guid OrderId) : IRequest<Result>;
}
