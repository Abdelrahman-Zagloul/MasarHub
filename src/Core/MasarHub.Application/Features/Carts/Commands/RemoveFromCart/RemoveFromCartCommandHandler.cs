using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Carts.Commands.RemoveFromCart
{
    public sealed class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, Result>
    {
        private readonly ICartService _cartService;

        public RemoveFromCartCommandHandler(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<Result> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
        {
            return await _cartService.RemoveItemAsync(request.UserId, request.CourseId, cancellationToken);
        }
    }
}