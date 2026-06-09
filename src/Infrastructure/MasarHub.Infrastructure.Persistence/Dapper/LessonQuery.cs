using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class LessonQuery : ILessonQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public LessonQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<LessonCreationData> GetCreationDataAsync(Guid courseId, Guid moduleId, Guid instructorId, CancellationToken ct = default)
        {
            const string sql = @"
            SELECT 
                CAST(CASE 
                    WHEN EXISTS (
                        SELECT 1 
                        FROM courses.CourseModules 
                        WHERE Id = @ModuleId AND CourseId = @CourseId AND IsDeleted = 0
                    ) THEN 1 
                    ELSE 0 
                END AS BIT) AS ModuleExist,

                CAST(CASE 
                    WHEN EXISTS (
                        SELECT 1 
                        FROM courses.Courses 
                        WHERE Id = @CourseId AND InstructorId = @InstructorId AND IsDeleted = 0
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
            var command = new CommandDefinition(sql, new { courseId, moduleId, instructorId }, cancellationToken: ct);

            var result = await connection.QuerySingleOrDefaultAsync<LessonCreationData>(command);
            return result ?? new LessonCreationData(false, false, 1);
        }
    }
}
