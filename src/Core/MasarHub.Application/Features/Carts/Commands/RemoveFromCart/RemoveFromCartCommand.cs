using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Carts.Commands.RemoveFromCart
{
    public sealed record RemoveFromCartCommand(Guid CourseId, Guid UserId) : IRequest<Result>;
}