using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Coupons.Queries.GetCoupons;
using MasarHub.Domain.Modules.Payments;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class CouponQuery : ICouponQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CouponQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<CreateCouponData> GetCreateCouponDataAsync(string code, Guid courseId, Guid instructorId, CancellationToken ct)
        {
            const string sql = @"
               SELECT
                CAST(CASE WHEN EXISTS (
                    SELECT 1
                    FROM payments.Coupons
                    WHERE Code = @Code And CourseId = @CourseId
                ) THEN 1 ELSE 0 END AS bit) AS CodeExists,


                CAST(CASE WHEN c.Id IS NOT NULL THEN 1 ELSE 0 END AS bit) AS CourseExists,

                CAST(CASE WHEN c.InstructorId = @InstructorId THEN 1 ELSE 0 END AS bit) AS IsOwner
                    FROM (VALUES (1)) v(x)
                    LEFT JOIN courses.Courses c
                    ON c.Id = @CourseId AND c.IsDeleted = 0;
                ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { Code = code, CourseId = courseId, InstructorId = instructorId }, cancellationToken: ct);
            return await connection.QuerySingleAsync<CreateCouponData>(command);
        }

        public async Task<CouponData?> GetCouponDataAsync(Guid couponId, Guid instructorId, CancellationToken ct)
        {
            const string sql = @"
                SELECT
                    c.CourseId,
                    CASE WHEN co.InstructorId = @InstructorId THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsOwner
                FROM payments.Coupons c
                INNER JOIN courses.Courses co ON c.CourseId = co.Id
                WHERE c.Id = @Id;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { Id = couponId, InstructorId = instructorId }, cancellationToken: ct);
            return await connection.QueryFirstOrDefaultAsync<CouponData>(command);
        }

        public async Task<CouponListResult> GetAllAsync(GetCouponsQuery query, Guid userId, CancellationToken ct)
        {
            var conditions = new List<string> { "c.CourseId = @CourseId" };
            var parameters = new DynamicParameters();
            parameters.Add("CourseId", query.CourseId);
            parameters.Add("UserId", userId);

            if (query.Status.HasValue)
            {
                switch (query.Status)
                {
                    case CouponStatus.Active:
                        conditions.Add("c.ExpirationDate > SYSUTCDATETIME() AND c.UsedCount < c.UsageLimit");
                        break;
                    case CouponStatus.Expired:
                        conditions.Add("c.ExpirationDate <= SYSUTCDATETIME()");
                        break;
                    case CouponStatus.Exhausted:
                        conditions.Add("c.UsedCount >= c.UsageLimit");
                        break;
                }
            }

            string couponWhere = "WHERE " + string.Join(" AND ", conditions);

            string sql = $@"
                SELECT 
                    CAST(1 AS BIT) AS CourseExists,
                    CAST(CASE WHEN InstructorId = @UserId THEN 1 ELSE 0 END AS BIT) AS IsOwner
                FROM courses.Courses
                WHERE Id = @CourseId AND IsDeleted = 0;

                SELECT 
                    Id, Code, Value, Type, CourseId,ExpirationDate, UsageLimit, UsedCount, CreatedAt
                FROM payments.Coupons c
                {couponWhere}
                ORDER BY c.CreatedAt DESC;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var access = await multi.ReadFirstOrDefaultAsync<(bool CourseExists, bool IsOwner)?>();
            if (access is null)
                return new CouponListResult([], false, false);

            var (courseExists, isOwner) = access.Value;
            var coupons = (await multi.ReadAsync<CouponResponse>()).ToList();

            return new CouponListResult(coupons, courseExists, isOwner);
        }
    }
}
