using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Carts.Commands.AddToCart
{
    public sealed record AddToCartCommand(Guid CourseId, Guid UserId) : IRequest<Result>;
}