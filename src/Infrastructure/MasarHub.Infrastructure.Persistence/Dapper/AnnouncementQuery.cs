using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent;
using MasarHub.Application.Features.Announcements.Queries.GetInstructorCourseAnnouncements;
using MasarHub.Application.Features.Announcements.Queries.GetStudentCourseAnnouncements;

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

        public async Task<PagedResult<InstructorCourseAnnouncementResponse>> GetInstructorListAsync(GetInstructorCourseAnnouncementsQuery query, CancellationToken ct)
        {
            var conditions = new List<string> { "ca.IsDeleted = 0" };
            var parameters = new DynamicParameters();

            parameters.Add("CourseId", query.CourseId);
            conditions.Add("ca.CourseId = @CourseId");

            parameters.Add("InstructorId", query.InstructorId);
            conditions.Add("ca.InstructorId = @InstructorId");

            if (query.Importance.HasValue)
            {
                parameters.Add("Importance", query.Importance.Value.ToString());
                conditions.Add("ca.Importance = @Importance");
            }

            if (query.IsPublished.HasValue)
            {
                parameters.Add("IsPublished", query.IsPublished.Value);
                conditions.Add("ca.IsPublished = @IsPublished");
            }

            if (query.IsPinned.HasValue)
            {
                parameters.Add("IsPinned", query.IsPinned.Value);
                conditions.Add("ca.IsPinned = @IsPinned");
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                parameters.Add("Search", $"%{query.Search}%");
                conditions.Add("(ca.Title LIKE @Search OR ca.Content LIKE @Search)");
            }

            string whereClause = "WHERE " + string.Join(" AND ", conditions);

            string sql = $@"
                SELECT COUNT(1)
                FROM courses.CourseAnnouncements ca
                {whereClause};

                SELECT
                    ca.Id,
                    ca.CourseId,
                    ca.InstructorId,
                    ca.Title,
                    ca.Content,
                    ca.IsPublished,
                    ca.PublishedAt,
                    ca.ScheduledAt,
                    ca.ExpiresAt,
                    ca.Importance,
                    ca.IsPinned,
                    ca.CreatedAt,
                    ca.UpdatedAt
                FROM courses.CourseAnnouncements ca
                {whereClause}
                ORDER BY ca.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            int offset = (query.PageNumber - 1) * query.PageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var totalCount = await multi.ReadFirstAsync<int>();
            var items = (await multi.ReadAsync<InstructorCourseAnnouncementResponse>()).ToList();
            return new PagedResult<InstructorCourseAnnouncementResponse>(items, totalCount);
        }

        public async Task<StudentAnnouncementPaginatedResult> GetStudentListAsync(GetStudentCourseAnnouncementsQuery query, CancellationToken ct)
        {
            var conditions = new List<string>
            {
                "ca.IsDeleted = 0",
                "ca.IsPublished = 1",
                "ca.CourseId = @CourseId"
            };
            var parameters = new DynamicParameters();

            parameters.Add("CourseId", query.CourseId);
            parameters.Add("StudentId", query.StudentId);

            if (query.Importance.HasValue)
            {
                parameters.Add("Importance", query.Importance.Value.ToString());
                conditions.Add("ca.Importance = @Importance");
            }

            if (query.IsPinned.HasValue)
            {
                parameters.Add("IsPinned", query.IsPinned.Value);
                conditions.Add("ca.IsPinned = @IsPinned");
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                parameters.Add("Search", $"%{query.Search}%");
                conditions.Add("(ca.Title LIKE @Search OR ca.Content LIKE @Search)");
            }

            string whereClause = "WHERE " + string.Join(" AND ", conditions);

            int offset = (query.PageNumber - 1) * query.PageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            string sql = $@"
               -- Check if the student is enrolled in the course
               SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM courses.CourseEnrollments
                    WHERE CourseId = @CourseId AND UserId = @StudentId AND IsDeleted = 0 AND Status = 'Active'
                ) THEN 1 ELSE 0 END AS BIT);

               
                SELECT COUNT(1)
                FROM courses.CourseAnnouncements ca
                {whereClause};

                SELECT
                    ca.Id,
                    ca.CourseId,
                    ca.Title,
                    ca.Content,
                    ca.PublishedAt,
                    ca.Importance,
                    ca.IsPinned,
                    ca.CreatedAt
                FROM courses.CourseAnnouncements ca
                {whereClause}
                ORDER BY ca.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var isEnrolled = await multi.ReadFirstAsync<bool>();
            if (!isEnrolled)
                return new StudentAnnouncementPaginatedResult(false, null);

            var totalCount = await multi.ReadFirstAsync<int>();
            var items = (await multi.ReadAsync<StudentCourseAnnouncementResponse>()).ToList();
            var pagedResult = new PagedResult<StudentCourseAnnouncementResponse>(items, totalCount);

            return new StudentAnnouncementPaginatedResult(true, pagedResult);
        }

    }
}
