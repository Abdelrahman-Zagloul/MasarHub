using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent;

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

        public async Task<StudentAnnouncementResult> GetByIdForStudentAsync(Guid courseId, Guid announcementId, Guid studentId, CancellationToken ct)
        {
            const string sql = @"
                SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM courses.CourseEnrollments
                    WHERE CourseId = @CourseId AND UserId = @StudentId AND IsDeleted = 0 AND Status = 'Active'
                ) THEN 1 ELSE 0 END AS BIT);

                SELECT
                    Id,
                    CourseId,
                    Title,
                    Content,
                    PublishedAt,
                    Importance,
                    IsPinned,
                    CreatedAt
                FROM courses.CourseAnnouncements
                WHERE Id = @AnnouncementId AND CourseId = @CourseId AND IsPublished = 1 AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { AnnouncementId = announcementId, CourseId = courseId, StudentId = studentId }, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var isEnrolled = await multi.ReadSingleAsync<bool>();
            if (!isEnrolled)
                return new StudentAnnouncementResult(false, null);

            var announcement = await multi.ReadFirstOrDefaultAsync<StudentCourseAnnouncementResponse>();
            return new StudentAnnouncementResult(isEnrolled, announcement);
        }
    }
}
