using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Courses.Queries.GetCourseById;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class CourseQuery : ICourseQuery
    {

        private readonly IDbConnectionFactory _connectionFactory;

        public CourseQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(bool CategoryExists, int SlugCount)> GetCreationDataAsync(string slug, Guid categoryId, CancellationToken ct = default)
        {
            const string sql = @"
                -- Check if the category exists
                SELECT CASE
                    WHEN EXISTS (
                        SELECT 1
                        FROM categories.Categories
                        WHERE Id = @CategoryId
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;

                -- Get count of matching slugs
                SELECT COUNT(*)
                FROM courses.Courses
                WHERE (
                        Slug = @Slug
                        OR Slug LIKE @Slug + '-%'
                      )
                  AND IsDeleted = 0;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { Slug = slug, CategoryId = categoryId, }, cancellationToken: ct);

            using var multi = await connection.QueryMultipleAsync(command);
            return (await multi.ReadFirstAsync<bool>(), await multi.ReadFirstAsync<int>());
        }
        public async Task<(string FullName, string Email)> GetInstructorInfoAsync(Guid instructorId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT 
                    FullName, 
                    Email
                FROM [identity].[Users]
                WHERE Id = @Id;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { Id = instructorId }, cancellationToken: ct);

            return await connection.QueryFirstOrDefaultAsync<(string FullName, string Email)>(command);
        }
        public async Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT CASE
                    WHEN EXISTS (
                        SELECT 1
                        FROM categories.Categories
                        WHERE Id = @CategoryId
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;
            ";
            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { CategoryId = categoryId, }, cancellationToken: ct);
            return await connection.ExecuteScalarAsync<bool>(command);
        }
        public async Task<bool> HasLecturesAsync(Guid courseId, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT CASE 
                    WHEN EXISTS (
                        SELECT 1 
                        FROM courses.CourseModules m
                        INNER JOIN courses.Lessons l ON m.Id = l.ModuleId
                        WHERE m.CourseId = @CourseId 
                          AND m.IsDeleted = 0 
                          AND l.IsDeleted = 0
                    ) THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { CourseId = courseId }, cancellationToken: cancellationToken);
            return await connection.ExecuteScalarAsync<bool>(command);
        }
        public async Task<CourseDetailsResponse?> GetDetailsByIdAsync(Guid courseId, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT 
                    c.Id, 
                    c.Title, 
                    c.Slug, 
                    c.Description, 
                    c.Price, 
                    c.Language, 
                    c.Status, 
                    c.Level, 
                    c.PublishedAt, 
                    c.InstructorId, 
                    u.FullName AS InstructorName, 
                    c.CategoryId, 
                    cat.Name AS CategoryName,
                    c.RejectionReason
                FROM courses.Courses c
                LEFT JOIN [identity].[Users] u ON c.InstructorId = u.Id
                LEFT JOIN categories.Categories cat ON c.CategoryId = cat.Id
                WHERE c.Id = @CourseId AND c.IsDeleted = 0;

                SELECT Value FROM courses.CoursePrerequisites WHERE CourseId = @CourseId;
                SELECT Value FROM courses.CourseRequirements WHERE CourseId = @CourseId;
                SELECT Value FROM courses.CourseLearningObjectives WHERE CourseId = @CourseId;
            ";


            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { CourseId = courseId }, cancellationToken: cancellationToken);
            using var multi = await connection.QueryMultipleAsync(command);

            var course = await multi.ReadFirstOrDefaultAsync<CourseDetailsResponse>();
            if (course is null)
                return null;

            course.Prerequisites = (await multi.ReadAsync<string>()).ToList();
            course.Requirements = (await multi.ReadAsync<string>()).ToList();
            course.LearningObjectives = (await multi.ReadAsync<string>()).ToList();
            return course;
        }
    }
}
