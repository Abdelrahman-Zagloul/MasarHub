using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Carts.Queries.GetCart
{
    public sealed record GetCartQuery(Guid UserId) : IRequest<Result<CartResponse>>;
}