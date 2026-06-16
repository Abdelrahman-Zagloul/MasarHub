using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class ExamQuery : IExamQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ExamQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ExamCreationData> GetCreationDataAsync(Guid courseId, Guid? moduleId, Guid instructorId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT 
                    CAST(CASE WHEN c.Id IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS CourseExists,
                    CAST(CASE WHEN c.InstructorId = @InstructorId THEN 1 ELSE 0 END AS BIT) AS IsOwner,
                    CAST(CASE 
                        WHEN @ModuleId IS NULL THEN 1
                        WHEN m.Id IS NOT NULL THEN 1
                        ELSE 0 
                    END AS BIT) AS ModuleExists
                FROM (SELECT 1 AS dummy) d
                LEFT JOIN courses.Courses c ON c.Id = @CourseId AND c.IsDeleted = 0
                LEFT JOIN courses.CourseModules m ON m.Id = @ModuleId AND m.CourseId = @CourseId AND m.IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();

            var command = new CommandDefinition(sql, new { courseId, moduleId, instructorId }, cancellationToken: ct);
            return await connection.QuerySingleAsync<ExamCreationData>(command);
        }
    }
}
