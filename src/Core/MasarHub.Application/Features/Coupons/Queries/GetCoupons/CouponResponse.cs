using MasarHub.Domain.Modules.Payments;

namespace MasarHub.Application.Features.Coupons.Queries.GetCoupons
{
    public sealed class CouponResponse
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = null!;
        public decimal Value { get; init; }
        public DiscountType Type { get; init; }
        public Guid CourseId { get; init; }
        public DateTimeOffset ExpirationDate { get; init; }
        public int UsageLimit { get; init; }
        public int UsedCount { get; init; }
        public DateTimeOffset CreatedAt { get; init; }

        public bool IsExpired => ExpirationDate <= DateTimeOffset.UtcNow;
        public bool IsExhausted => UsedCount >= UsageLimit;
        public bool IsActive => !IsExpired && !IsExhausted;
    }
}
