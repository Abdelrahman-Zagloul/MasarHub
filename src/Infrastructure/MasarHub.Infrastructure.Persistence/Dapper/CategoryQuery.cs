using Dapper;
using MasarHub.Application.Abstractions.Queries;
using MasarHub.Domain.Modules.Categories;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class CategoryQuery : ICategoryQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CategoryQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> CategoryExistsAsync(Guid id, CancellationToken ct)
        {
            const string sql = @"
                SELECT CASE
                    WHEN EXISTS (
                        SELECT 1
                        FROM categories.Categories
                        WHERE Id = @Id
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: ct);
            return await connection.ExecuteScalarAsync<bool>(command);
        }
        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            const string sql = @"
                SELECT *
                FROM categories.Categories
                WHERE Id = @Id;";

            using var connection = _connectionFactory.CreateConnection();

            var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: ct);

            return await connection.QueryFirstOrDefaultAsync<Category>(command);

        }
        public async Task<(int DisplayOrder, bool SlugExists)> GetCreationDataAsync(string slug, Guid? parentCategoryId, CancellationToken ct)
        {
            const string sql = @"
                -- DisplayOrder
                SELECT COALESCE(MAX(DisplayOrder), 0) + 1
                FROM categories.Categories
                WHERE (@ParentCategoryId IS NULL AND ParentCategoryId IS NULL)
                    OR ParentCategoryId = @ParentCategoryId;

                -- Slug Exists
                SELECT CASE
                    WHEN EXISTS (
                        SELECT 1
                        FROM categories.Categories
                        WHERE Slug = @Slug
                            AND (
                                (@ParentCategoryId IS NULL AND ParentCategoryId IS NULL)
                                OR ParentCategoryId = @ParentCategoryId
                            )
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;
            ";

            using var connection = _connectionFactory.CreateConnection();

            var command = new CommandDefinition(
                sql,
                new
                {
                    Slug = slug,
                    ParentCategoryId = parentCategoryId
                },
                cancellationToken: ct);

            using var multi = await connection.QueryMultipleAsync(command);

            return (await multi.ReadFirstAsync<int>(), await multi.ReadFirstAsync<bool>());
        }
    }
}
