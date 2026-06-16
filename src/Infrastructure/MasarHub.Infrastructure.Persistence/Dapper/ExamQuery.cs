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

        public async Task<ExamUpdateData> GetUpdateDataAsync(Guid examId, Guid instructorId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT
                    CAST(1 AS BIT) AS ExamExists,
                    CAST(CASE WHEN c.InstructorId = @InstructorId THEN 1 ELSE 0 END AS BIT) AS IsOwner
                FROM exams.Exams e
                INNER JOIN courses.Courses c ON c.Id = e.CourseId AND c.IsDeleted = 0
                WHERE e.Id = @ExamId AND e.IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();

            var command = new CommandDefinition(sql, new { examId, instructorId }, cancellationToken: ct);
            return await connection.QuerySingleOrDefaultAsync<ExamUpdateData>(command) ?? new ExamUpdateData(false, false);
        }

        public async Task<ExamDeleteData> GetDeleteDataAsync(Guid examId, Guid instructorId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT
                    CAST(1 AS BIT) AS ExamExists,
                    CAST(CASE WHEN c.InstructorId = @InstructorId THEN 1 ELSE 0 END AS BIT) AS IsOwner,
                    CAST(CASE WHEN EXISTS (
                        SELECT 1 FROM exams.ExamAttempts ea
                        WHERE ea.ExamId = e.Id 
                            AND ea.Status = 'Submitted' 
                            AND ea.IsDeleted = 0
                    ) THEN 1 ELSE 0 END AS BIT) AS HasAttempts
                FROM exams.Exams e
                LEFT JOIN courses.Courses c ON c.Id = e.CourseId AND c.IsDeleted = 0
                WHERE e.Id = @ExamId AND e.IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();

            var command = new CommandDefinition(sql, new { examId, instructorId }, cancellationToken: ct);
            var result = await connection.QuerySingleOrDefaultAsync<ExamDeleteData>(command);
            return result ?? new ExamDeleteData(false, false, false);
        }
    }
}
