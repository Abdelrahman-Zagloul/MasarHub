using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Carts.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MasarHub.Infrastructure.Services
{
    public sealed class CartService : ICartService
    {
        private readonly ICacheService _cache;
        private static readonly TimeSpan CartExpiry = TimeSpan.FromDays(7);

        public CartService([FromKeyedServices("redis")] ICacheService cache)
        {
            _cache = cache;
        }

        public async Task<Cart> GetOrCreateAsync(Guid userId, CancellationToken ct = default)
        {
            var cart = await _cache.GetAsync<Cart>(CartKey(userId), ct);
            if (cart != null)
                return cart;

            return new Cart(userId);
        }

        public async Task<Result> AddItemAsync(Guid userId, CartItem item, CancellationToken ct = default)
        {
            var cart = await GetOrCreateAsync(userId, ct);

            var result = cart.AddItem(item);
            if (result.IsFailure)
                return result;

            await _cache.SetAsync(CartKey(userId), cart, CartExpiry, ct);
            return Result.Success();
        }

        public async Task<Result> RemoveItemAsync(Guid userId, Guid courseId, CancellationToken ct = default)
        {
            var cart = await GetOrCreateAsync(userId, ct);

            var result = cart.RemoveItem(courseId);
            if (result.IsFailure)
                return result;

            await _cache.SetAsync(CartKey(userId), cart, CartExpiry, ct);
            return Result.Success();
        }

        public async Task ClearAsync(Guid userId, CancellationToken ct = default)
        {
            await _cache.RemoveAsync(CartKey(userId), ct);
        }

        private static string CartKey(Guid userId) => $"cart:{userId}";
    }
}