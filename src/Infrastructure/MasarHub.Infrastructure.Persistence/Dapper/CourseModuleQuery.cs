using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class CourseModuleQuery : ICourseModuleQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public CourseModuleQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(bool CourseExists, bool IsOwner, int NextDisplayOrder)> GetCreationDataAsync(
            Guid courseId, Guid instructorId, CancellationToken cancellationToken)
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
                cancellationToken: cancellationToken);

            return await connection.QueryFirstAsync<(bool CourseExists, bool IsOwner, int NextDisplayOrder)>(command);
        }

        public async Task<(bool ModuleExists, bool IsOwner, Guid CourseId)> GetUpdateDataAsync(
            Guid moduleId, Guid instructorId, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT 
                    CAST(CASE WHEN m.Id IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS ModuleExists,
                    CAST(CASE WHEN c.InstructorId = @InstructorId THEN 1 ELSE 0 END AS BIT) AS IsOwner,
                    m.CourseId AS CourseId
                FROM courses.CourseModules m
                INNER JOIN courses.Courses c ON m.CourseId = c.Id AND c.IsDeleted = 0
                WHERE m.Id = @ModuleId AND m.IsDeleted = 0;
            ";

            using var connection = _connectionFactory.CreateConnection();

            var command = new CommandDefinition(sql, new { ModuleId = moduleId, InstructorId = instructorId }, cancellationToken: cancellationToken);

            var result = await connection.QueryFirstOrDefaultAsync<(bool ModuleExists, bool IsOwner, Guid CourseId)>(command);
            return result;
        }

        public async Task<bool> IsCourseOwnerAsync(Guid courseId, Guid instructorId, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT CAST(
                    CASE 
                        WHEN EXISTS (
                            SELECT 1 
                            FROM courses.Courses 
                            WHERE Id = @CourseId AND InstructorId = @InstructorId AND IsDeleted = 0
                        ) THEN 1 
                        ELSE 0 
                    END AS BIT);
            ";
            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { CourseId = courseId, InstructorId = instructorId }, cancellationToken: cancellationToken);
            return await connection.ExecuteScalarAsync<bool>(command);
        }
    }
}
