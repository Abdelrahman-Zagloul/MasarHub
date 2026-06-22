using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ICouponQuery : IScopedService
    {
        Task<CreateCouponData> GetCreateCouponDataAsync(string code, Guid courseId, Guid instructorId, CancellationToken ct = default);
        Task<CouponData?> GetCouponDataAsync(Guid couponId, Guid instructorId, CancellationToken ct = default);
    }

    public sealed record CreateCouponData(bool CodeExists, bool CourseExists, bool IsOwner);
    public sealed record CouponData(Guid CourseId, bool IsOwner);
}
