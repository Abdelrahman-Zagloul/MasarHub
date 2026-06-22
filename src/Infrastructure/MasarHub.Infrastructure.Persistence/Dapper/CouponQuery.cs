using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;

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

        public async Task<DeleteCouponData?> GetDeleteCouponDataAsync(Guid couponId, Guid instructorId, CancellationToken ct)
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
            return await connection.QueryFirstOrDefaultAsync<DeleteCouponData>(command);
        }
    }
}
