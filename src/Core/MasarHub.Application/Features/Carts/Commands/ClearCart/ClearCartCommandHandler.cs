using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Carts.Commands.ClearCart
{
    public sealed class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Result>
    {
        private readonly ICartService _cartService;

        public ClearCartCommandHandler(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            await _cartService.ClearAsync(request.UserId, cancellationToken);
            return Result.Success();
        }
    }
}