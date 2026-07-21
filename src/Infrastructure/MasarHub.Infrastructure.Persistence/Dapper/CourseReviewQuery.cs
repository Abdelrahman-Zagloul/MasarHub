using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class CourseReviewQuery : ICourseReviewQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CourseReviewQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<CreateReviewCheckResult> GetCreateReviewCheckAsync(Guid courseId, Guid userId, CancellationToken ct)
        {
            const string sql = @"
                SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM courses.CourseEnrollments
                    WHERE CourseId = @CourseId AND UserId = @UserId AND IsDeleted = 0 AND Status = 'Active'
                ) THEN 1 ELSE 0 END AS BIT);

                SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM courses.CourseReviews
                    WHERE CourseId = @CourseId AND UserId = @UserId AND IsDeleted = 0
                ) THEN 1 ELSE 0 END AS BIT)";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { CourseId = courseId, UserId = userId }, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var isEnrolled = await multi.ReadSingleAsync<bool>();
            var hasExistingReview = await multi.ReadSingleAsync<bool>();

            return new CreateReviewCheckResult(isEnrolled, hasExistingReview);
        }
    }
}
