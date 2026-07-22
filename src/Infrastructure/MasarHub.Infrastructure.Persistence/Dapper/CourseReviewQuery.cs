using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Reviews.Queries.GetCourseReviewById;
using MasarHub.Application.Features.Reviews.Queries.GetCourseReviews;

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

        public async Task<CourseReviewResponse?> GetByIdAsync(Guid courseId, Guid reviewId, CancellationToken ct)
        {
            const string sql = @"
                SELECT
                    r.Id,
                    r.CourseId,
                    r.UserId,
                    u.FullName,
                    CAST(r.Rating AS FLOAT) AS Rating,
                    r.ReviewContent,
                    r.CreatedAt,
                    r.EditedAt
                FROM courses.CourseReviews r
                INNER JOIN [identity].Users u ON r.UserId = u.Id
                WHERE r.Id = @ReviewId AND r.CourseId = @CourseId AND r.IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { ReviewId = reviewId, CourseId = courseId }, cancellationToken: ct);
            return await connection.QueryFirstOrDefaultAsync<CourseReviewResponse>(command);
        }

        public async Task<PagedResult<CourseReviewResponse>> GetAllAsync(GetCourseReviewsQuery query, CancellationToken ct)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM courses.CourseReviews r
                WHERE r.CourseId = @CourseId AND r.IsDeleted = 0;

                SELECT
                    r.Id,
                    r.CourseId,
                    r.UserId,
                    u.FullName,
                    CAST(r.Rating AS FLOAT) AS Rating,
                    r.ReviewContent,
                    r.CreatedAt,
                    r.EditedAt
                FROM courses.CourseReviews r
                INNER JOIN [identity].Users u ON r.UserId = u.Id
                WHERE r.CourseId = @CourseId AND r.IsDeleted = 0
                ORDER BY r.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var parameters = new DynamicParameters();
            parameters.Add("CourseId", query.CourseId);
            parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var totalCount = await multi.ReadFirstAsync<int>();
            var reviews = (await multi.ReadAsync<CourseReviewResponse>()).ToList();

            return new PagedResult<CourseReviewResponse>(reviews, totalCount);
        }
    }
}
