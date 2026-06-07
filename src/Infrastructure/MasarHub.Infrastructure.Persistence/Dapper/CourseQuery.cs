using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;

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
    }
}
