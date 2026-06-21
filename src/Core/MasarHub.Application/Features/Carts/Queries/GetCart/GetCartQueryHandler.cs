using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Carts.Queries.GetCart
{
    public sealed class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<CartResponse>>
    {
        private readonly ICartService _cartService;

        public GetCartQueryHandler(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<Result<CartResponse>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartService.GetOrCreateAsync(request.UserId, cancellationToken);

            return new CartResponse(cart.Items, cart.Items.Count);
        }
    }
}