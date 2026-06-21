using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Carts.Models;

namespace MasarHub.Application.Abstractions.Services
{
    public interface ICartService : IScopedService
    {
        Task<Cart> GetOrCreateAsync(Guid userId, CancellationToken ct = default);
        Task<Result> AddItemAsync(Guid userId, CartItem item, CancellationToken ct = default);
        Task<Result> RemoveItemAsync(Guid userId, Guid courseId, CancellationToken ct = default);
        Task ClearAsync(Guid userId, CancellationToken ct = default);
    }
}