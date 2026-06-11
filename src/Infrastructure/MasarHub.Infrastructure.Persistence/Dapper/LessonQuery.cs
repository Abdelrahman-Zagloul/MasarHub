using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class LessonQuery : ILessonQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public LessonQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<LessonCreationData> GetCreationDataAsync(Guid moduleId, Guid instructorId, CancellationToken ct = default)
        {
            const string sql = @"
            SELECT 
                CAST(CASE 
                    WHEN EXISTS (
                        SELECT 1 
                        FROM courses.CourseModules 
                        WHERE Id = @ModuleId AND IsDeleted = 0
                    ) THEN 1 
                    ELSE 0 
                END AS BIT) AS ModuleExist,

                CAST(CASE 
                    WHEN EXISTS (
                        SELECT 1
                        FROM courses.CourseModules cm
                        INNER JOIN courses.Courses c
                            ON c.Id = cm.CourseId
                        WHERE cm.Id = @ModuleId
                            AND cm.IsDeleted = 0
                            AND c.IsDeleted = 0
                            AND c.InstructorId = @InstructorId
                    ) THEN 1 
                    ELSE 0 
                END AS BIT) AS IsOwner,

                ISNULL((
                    SELECT MAX(DisplayOrder) 
                    FROM courses.Lessons 
                    WHERE ModuleId = @ModuleId AND IsDeleted = 0
                ), 0) + 1 AS NextDisplayOrder;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { moduleId, instructorId }, cancellationToken: ct);
            var result = await connection.QuerySingleOrDefaultAsync<LessonCreationData>(command);
            return result ?? new LessonCreationData(false, false, 0);
        }
        public async Task<LessonAttachmentCreationData> GetLessonAttachmentCreationAsync(Guid lessonId, Guid instructorId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT
                    CAST(CASE
                        WHEN EXISTS (
                            SELECT 1
                            FROM courses.Lessons l
                            WHERE l.Id = @LessonId
                                AND l.IsDeleted = 0
                        )
                        THEN 1 ELSE 0
                    END AS BIT) AS LessonExist,

                    CAST(CASE
                        WHEN EXISTS (
                            SELECT 1
                            FROM courses.Lessons l
                            INNER JOIN courses.CourseModules m
                                ON m.Id = l.ModuleId
                            INNER JOIN courses.Courses c
                                ON c.Id = m.CourseId
                            WHERE l.Id = @LessonId
                                AND c.InstructorId = @InstructorId
                                AND l.IsDeleted = 0
                                AND m.IsDeleted = 0
                                AND c.IsDeleted = 0
                        )
                        THEN 1 ELSE 0
                    END AS BIT) AS IsOwner,

                    (
                        SELECT COUNT(*)
                        FROM courses.LessonAttachments
                        WHERE LessonId = @LessonId
                            AND IsDeleted = 0
                    ) AS AttachmentCount;
                ";

            using var connection = _connectionFactory.CreateConnection();

            var command = new CommandDefinition(sql, new { lessonId, instructorId }, cancellationToken: ct);
            var result = await connection.QuerySingleOrDefaultAsync<LessonAttachmentCreationData>(command);
            return result ?? new LessonAttachmentCreationData(false, false, 0);
        }
        public async Task<CourseState> GetCourseStateAsync(Guid moduleId, Guid instructorId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT
                    CAST(1 AS BIT) AS ModuleExist,

                    CAST(
                        CASE
                            WHEN c.InstructorId = @InstructorId
                            THEN 1
                            ELSE 0
                        END
                    AS BIT) AS IsOwner,

                    c.Status AS CourseStatus
                FROM courses.CourseModules cm
                INNER JOIN courses.Courses c
                    ON c.Id = cm.CourseId
                WHERE cm.Id = @ModuleId
                  AND cm.IsDeleted = 0
                  AND c.IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { moduleId, instructorId }, cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<CourseState>(command);
            return result ?? new CourseState(false, false, CourseStatus.Draft);
        }
        public async Task<LessonReorderData> GetReorderDataAsync(Guid moduleId, Guid instructorId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT
                    CAST(1 AS BIT) AS ModuleExist,

                    CAST(
                        CASE
                            WHEN c.InstructorId = @InstructorId
                            THEN 1
                            ELSE 0
                        END
                    AS BIT) AS IsOwner

                FROM courses.CourseModules m
                INNER JOIN courses.Courses c
                    ON c.Id = m.CourseId
                    AND c.IsDeleted = 0

                WHERE m.Id = @ModuleId
                    AND m.IsDeleted = 0;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new
            {
                ModuleId = moduleId,
                InstructorId = instructorId
            }, cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<LessonReorderData>(command);
            return result ?? new LessonReorderData(false, false);
        }
        public async Task<bool> IsLessonOwnedByInstructorAsync(Guid lessonId, Guid instructorId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT CAST(CASE
                    WHEN EXISTS (
                        SELECT 1
                        FROM courses.Lessons l
                        INNER JOIN courses.CourseModules m
                            ON m.Id = l.ModuleId
                        INNER JOIN courses.Courses c
                            ON c.Id = m.CourseId
                        WHERE l.Id = @LessonId
                            AND c.InstructorId = @InstructorId
                            AND l.IsDeleted = 0
                            AND m.IsDeleted = 0
                            AND c.IsDeleted = 0
                    )
                    THEN 1 ELSE 0
                END AS BIT);
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { lessonId, instructorId }, cancellationToken: ct);

            return await connection.QuerySingleAsync<bool>(command);
        }
    }
}
