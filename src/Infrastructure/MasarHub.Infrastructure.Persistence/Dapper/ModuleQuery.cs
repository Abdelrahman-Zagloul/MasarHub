using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class ModuleQuery : IModuleQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public ModuleQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(bool CourseExists, bool IsOwner, int NextDisplayOrder)> GetCreationDataAsync(
            Guid courseId, Guid instructorId, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT 
                    CASE 
                        WHEN EXISTS (
                            SELECT 1 FROM courses.Courses WHERE Id = @CourseId AND IsDeleted = 0
                        ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) 
                    END AS CourseExists,
            
                    CASE 
                        WHEN EXISTS (
                            SELECT 1 FROM courses.Courses WHERE Id = @CourseId AND InstructorId = @InstructorId AND IsDeleted = 0
                        ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) 
                    END AS IsOwner;

                SELECT COALESCE(
                    (SELECT MAX(DisplayOrder) + 1 FROM courses.CourseModules WHERE CourseId = @CourseId AND IsDeleted = 0),
                    1
                ) AS NextDisplayOrder;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(
                sql,
                new { CourseId = courseId, InstructorId = instructorId },
                cancellationToken: cancellationToken);

            using var multi = await connection.QueryMultipleAsync(command);
            var courseData = await multi.ReadFirstAsync<(bool CourseExists, bool IsOwner)>();
            var nextDisplayOrder = await multi.ReadFirstAsync<int>();
            return (courseData.CourseExists, courseData.IsOwner, nextDisplayOrder);
        }
    }
}
