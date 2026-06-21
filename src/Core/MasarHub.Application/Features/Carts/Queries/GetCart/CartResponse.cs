using MasarHub.Application.Features.Carts.Models;

namespace MasarHub.Application.Features.Carts.Queries.GetCart
{
    public sealed record CartResponse
    (
        IReadOnlyList<CartItem> Items,
        int TotalCount
    );
}