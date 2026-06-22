using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Features.Coupons.Queries.GetCoupons;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ICouponQuery : IScopedService
    {
        Task<CreateCouponData> GetCreateCouponDataAsync(string code, Guid courseId, Guid instructorId, CancellationToken ct = default);
        Task<CouponData?> GetCouponDataAsync(Guid couponId, Guid instructorId, CancellationToken ct = default);
        Task<CouponListResult> GetAllAsync(GetCouponsQuery query, Guid userId, CancellationToken ct = default);
    }

    public sealed record CreateCouponData(bool CodeExists, bool CourseExists, bool IsOwner);
    public sealed record CouponData(Guid CourseId, bool IsOwner);
    public sealed record CouponListResult(List<CouponResponse> Coupons, bool CourseExists, bool IsOwner);
}
