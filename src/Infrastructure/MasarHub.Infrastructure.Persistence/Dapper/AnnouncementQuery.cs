using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class AnnouncementQuery : IAnnouncementQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AnnouncementQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Guid>> GetEnrolledUserIdsAsync(Guid courseId, CancellationToken ct)
        {
            const string sql = @"
                SELECT UserId
                FROM courses.CourseEnrollments
                WHERE CourseId = @CourseId AND IsDeleted = 0 AND Status = 'Active'";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { CourseId = courseId }, cancellationToken: ct);
            return (await connection.QueryAsync<Guid>(command)).AsList();
        }

        public async Task<InstructorCourseAnnouncementResponse?> GetByIdForInstructorAsync(Guid courseId, Guid announcementId, Guid instructorId, CancellationToken ct)
        {
            const string sql = @"
                SELECT
                    Id,
                    CourseId,
                    InstructorId,
                    Title,
                    Content,
                    IsPublished,
                    PublishedAt,
                    ScheduledAt,
                    ExpiresAt,
                    Importance,
                    IsPinned,
                    CreatedAt,
                    UpdatedAt
                FROM courses.CourseAnnouncements
                WHERE Id = @AnnouncementId AND CourseId = @CourseId AND InstructorId = @InstructorId AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { AnnouncementId = announcementId, CourseId = courseId, InstructorId = instructorId }, cancellationToken: ct);
            return await connection.QueryFirstOrDefaultAsync<InstructorCourseAnnouncementResponse>(command);
        }
    }
}
