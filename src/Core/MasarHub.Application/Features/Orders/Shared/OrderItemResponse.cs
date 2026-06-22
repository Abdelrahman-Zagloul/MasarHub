namespace MasarHub.Application.Features.Orders.Shared
{
    public sealed record OrderItemResponse
    (
        Guid CourseId,
        string CourseTitle,
        decimal OriginalPrice,
        decimal DiscountAmount,
        decimal FinalPrice,
        Guid? CouponId
    );
}
