namespace MasarHub.Application.Features.Carts.Models
{
    public sealed class CartItem
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ThumbnailPublicId { get; set; }
        public DateTimeOffset AddedAt { get; set; }

        public CartItem() { }

        public static CartItem Create(Guid courseId, string title, decimal price, string? thumbnailPublicId)
            => new()
            {
                CourseId = courseId,
                Title = title,
                Price = price,
                ThumbnailPublicId = thumbnailPublicId,
                AddedAt = DateTimeOffset.UtcNow
            };
    }
}