using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Orders.Commands.DeleteOrder
{
    public sealed record DeleteOrderCommand(Guid UserId, Guid OrderId) : IRequest<Result>;
}
