using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;

namespace MasarHub.Application.Features.Carts.Models
{
    public sealed class Cart
    {
        // set as public for redis sterilize 

        public Guid UserId { get; set; }
        public List<CartItem> Items { get; set; } = [];
        public DateTimeOffset LastModifiedAt { get; set; }

        public Cart() { }

        public Cart(Guid userId)
        {
            UserId = userId;
            LastModifiedAt = DateTimeOffset.UtcNow;
        }

        public Result AddItem(CartItem item)
        {
            if (Items.Any(i => i.CourseId == item.CourseId))
                return Error.Conflict("cart.course_already_exists");

            Items.Add(item);
            LastModifiedAt = DateTimeOffset.UtcNow;
            return Result.Success();
        }

        public Result RemoveItem(Guid courseId)
        {
            var item = Items.FirstOrDefault(i => i.CourseId == courseId);
            if (item is null)
                return Error.NotFound("cart.course_not_found");

            Items.Remove(item);
            LastModifiedAt = DateTimeOffset.UtcNow;
            return Result.Success();
        }
    }
}