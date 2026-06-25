using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Orders.Commands.CreateOrder;
using MasarHub.Domain.Modules.Orders;
using MasarHub.Domain.Modules.Payments;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class OrderQuery : IOrderQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<CreateOrderData> GetCreateOrderDataAsync(Guid userId, List<Guid> courseIds, List<CourseCoupon>? courseCoupons, CancellationToken ct = default)
        {
            var couponCodes = courseCoupons?.Select(c => c.CouponCode).ToList() ?? [];
            var couponCourseIds = courseCoupons?.Select(c => c.CourseId).ToList() ?? [];

            const string sql = @"
                SELECT
                    Id, Title, Price, ThumbnailPublicId,
                    CAST(CASE WHEN Status = 'Published' THEN 1 ELSE 0 END AS BIT) AS IsPublished
                FROM courses.Courses
                WHERE Id IN @CourseIds AND IsDeleted = 0;

                SELECT Id, Code, Value, Type, CourseId, ExpirationDate, UsageLimit, UsedCount
                FROM payments.Coupons
                WHERE Code IN @CouponCodes AND CourseId IN @CouponCourseIds;

                SELECT CouponId
                FROM payments.CouponUsages
                WHERE UserId = @UserId AND CouponId IN (
                    SELECT Id FROM payments.Coupons WHERE Code IN @CouponCodes AND CourseId IN @CouponCourseIds
                );
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { UserId = userId, CourseIds = courseIds, CouponCodes = couponCodes, CouponCourseIds = couponCourseIds }, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var courses = (await multi.ReadAsync<CourseCartData>()).AsList();
            var coupons = (await multi.ReadAsync<Coupon>()).AsList();
            var usageIds = (await multi.ReadAsync<Guid>()).AsList();

            return new CreateOrderData(courses, coupons, usageIds);
        }
        public async Task<OrderInitiateData> GetOrderInitiateDataAsync(Guid orderId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT Id, UserId, Status, FinalAmount
                FROM orders.Orders
                WHERE Id = @Id AND IsDeleted = 0;

                SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM payments.Payments WHERE OrderId = @Id
                ) THEN 1 ELSE 0 END AS BIT);
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { Id = orderId }, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var order = await multi.ReadFirstOrDefaultAsync<Order>();
            var hasPayment = await multi.ReadFirstAsync<bool>();

            return new OrderInitiateData(order, hasPayment);
        }
    }
}
