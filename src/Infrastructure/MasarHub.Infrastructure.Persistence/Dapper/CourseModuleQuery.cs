using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Modules.Queries.GetModuleById;
using System.Text;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class CourseModuleQuery : ICourseModuleQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public CourseModuleQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ModuleCreationData> GetCreationDataAsync(Guid courseId, Guid instructorId, CancellationToken ct)
        {
            const string sql = @"
                SELECT 
                    CAST(CASE WHEN EXISTS (
                        SELECT 1 FROM courses.Courses WHERE Id = @CourseId AND IsDeleted = 0
                    ) THEN 1 ELSE 0 END AS BIT) AS CourseExists,

                    CAST(CASE WHEN EXISTS (
                        SELECT 1 FROM courses.Courses WHERE Id = @CourseId AND InstructorId = @InstructorId AND IsDeleted = 0
                    ) THEN 1 ELSE 0 END AS BIT) AS IsOwner,

                    COALESCE((SELECT MAX(DisplayOrder) + 1 FROM courses.CourseModules WHERE CourseId = @CourseId), 1) AS NextDisplayOrder;
            ";
            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(
                sql,
                new { CourseId = courseId, InstructorId = instructorId },
                cancellationToken: ct);

            return await connection.QueryFirstAsync<ModuleCreationData>(command);
        }
        public async Task<ModuleUpdateData> GetUpdateDataAsync(Guid courseId, Guid moduleId, Guid instructorId, CancellationToken ct)
        {
            const string sql = @"
                SELECT
                    CAST(1 AS BIT) AS ModuleExists,

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
                    AND m.CourseId = @CourseId
                    AND m.IsDeleted = 0;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(
                sql,
                new
                {
                    CourseId = courseId,
                    ModuleId = moduleId,
                    InstructorId = instructorId
                },
                cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<ModuleUpdateData>(command);
            return result ?? new ModuleUpdateData(false, false);
        }
        public async Task<ModuleDeleteData> GetDeleteDataAsync(Guid courseId, Guid moduleId, Guid instructorId, CancellationToken ct)
        {
            const string sql = @"
                SELECT
                    CAST(1 AS BIT) AS ModuleExists,

                    CAST(
                        CASE
                            WHEN c.InstructorId = @InstructorId
                            THEN 1
                            ELSE 0
                        END
                    AS BIT) AS IsOwner,

                    CAST(
                        CASE
                            WHEN EXISTS (
                                SELECT 1
                                FROM courses.Lessons l
                                WHERE l.ModuleId = m.Id
                                    AND l.IsDeleted = 0
                            )
                            THEN 1
                            ELSE 0
                        END
                    AS BIT) AS HasLessons

                FROM courses.CourseModules m
                INNER JOIN courses.Courses c
                    ON c.Id = m.CourseId
                    AND c.IsDeleted = 0

                WHERE m.Id = @ModuleId
                    AND m.CourseId = @CourseId
                    AND m.IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new
            {
                CourseId = courseId,
                ModuleId = moduleId,
                InstructorId = instructorId
            }, cancellationToken: ct);
            var result = await connection.QueryFirstOrDefaultAsync<ModuleDeleteData>(command);
            return result ?? new ModuleDeleteData(false, false, false);
        }
        public async Task<List<Guid>> GetModuleIdsByCourseIdAsync(Guid courseId, CancellationToken ct = default)
        {
            const string sql = "SELECT Id FROM courses.Modules WHERE CourseId = @CourseId AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { CourseId = courseId }, cancellationToken: ct);
            var ids = await connection.QueryAsync<Guid>(command);
            return ids.ToList();
        }
        public async Task<bool> BulkUpdateDisplayOrderAsync(Guid courseId, IReadOnlyList<Guid> orderedModuleIds, CancellationToken ct = default)
        {
            if (orderedModuleIds is null || orderedModuleIds.Count == 0)
                return false;

            var valuesBuilder = new StringBuilder(orderedModuleIds.Count * 45);
            for (int i = 0; i < orderedModuleIds.Count; i++)
                valuesBuilder.Append($"('{orderedModuleIds[i]}', {i + 1}),");

            valuesBuilder.Length--;

            var sql = $@"
                UPDATE M
                SET M.DisplayOrder = T.NewOrder,
                    M.UpdatedAt = SYSUTCDATETIME() 
                FROM courses.Modules M
                INNER JOIN (
                    VALUES {valuesBuilder.ToString()}
                ) AS T(ModuleId, NewOrder) ON M.Id = T.ModuleId
                WHERE M.CourseId = @CourseId AND M.IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { CourseId = courseId }, cancellationToken: ct);

            var affectedRows = await connection.ExecuteAsync(command);
            return affectedRows > 0;
        }
        public async Task<ModuleDetailsResponse?> GetModuleByIdAsync(Guid courseId, Guid moduleId, CancellationToken ct)
        {
            const string sql = @"
                SELECT
                    m.Id AS ModuleId,
                    m.Title,
                    m.Description,
                    m.DisplayOrder
                FROM courses.CourseModules m
                WHERE m.Id = @ModuleId AND m.CourseId = @CourseId AND m.IsDeleted = 0;

                SELECT
                    l.Id AS LessonId,
                    l.Title,
                    l.Description,
                    l.DisplayOrder,
                    l.IsPreviewable,
                    l.LessonType,
                    l.VideoPublicId
                FROM courses.Lessons l
                WHERE l.ModuleId = @ModuleId AND l.IsDeleted = 0 AND l.LessonStatus = 'Active'
                ORDER BY l.DisplayOrder;";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { CourseId = courseId, ModuleId = moduleId }, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var module = await multi.ReadFirstOrDefaultAsync<ModuleDetailsResponse>();
            if (module is null)
                return null;

            module.Lessons = (await multi.ReadAsync<LessonResponse>()).AsList();
            return module;
        }
    }
}
