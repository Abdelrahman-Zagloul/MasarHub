using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Categories.Queries.GetCategories;
using MasarHub.Application.Features.Categories.Queries.GetCategoryById;
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
        public async Task<bool> HasChildrenAsync(Guid id, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT CASE
                    WHEN EXISTS (
                        SELECT 1
                        FROM categories.Categories
                        WHERE ParentCategoryId = @Id
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;";

            var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: ct);
            return await connection.ExecuteScalarAsync<bool>(command);
        }
        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            const string sql = @"
                SELECT*
                FROM categories.Categories
                WHERE Id = @Id; ";

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
        public async Task<(bool hasChildren, bool hasCourses)> CanDeleteAsync(Guid id, CancellationToken ct = default)
        {
            const string sql = @"
                 -- Check for child categories
                 SELECT CASE
                     WHEN EXISTS (
                         SELECT 1
                         FROM categories.Categories
                         WHERE ParentCategoryId = @Id
                     )
                     THEN CAST(1 AS BIT)
                     ELSE CAST(0 AS BIT)
                 END;


                -- Check for associated courses
                SELECT CASE
                    WHEN EXISTS (
                        SELECT 1
                        FROM courses.Courses
                        WHERE CategoryId = @Id
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;
             ";
            using var connection = _connectionFactory.CreateConnection();

            var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);
            return (await multi.ReadFirstAsync<bool>(), await multi.ReadFirstAsync<bool>());
        }
        public async Task<CategoryWithChildrenResponse?> GetWithChildrenByIdAsync(Guid id, CancellationToken ct = default)
        {
            const string sql = @"
                -- Get the category
                SELECT Id, Name, Description, Slug, Level, DisplayOrder, ParentCategoryId, CreatedAt
                FROM categories.Categories
                WHERE Id = @Id;

                -- Get child categories
                SELECT Id, Name, Description, Slug, Level, DisplayOrder, ParentCategoryId, CreatedAt
                FROM categories.Categories
                WHERE ParentCategoryId = @Id
                ORDER BY DisplayOrder;";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var category = await multi.ReadFirstOrDefaultAsync<CategoryResponse>();
            if (category is null)
                return null;

            var children = (await multi.ReadAsync<CategoryResponse>()).ToList();
            return new CategoryWithChildrenResponse(category, children);
        }
        public async Task<(int TotalCount, List<CategoryResponse> Categories)> GetAllAsync(GetCategoriesQuery query, CancellationToken ct = default)
        {
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(query.CategoryName))
            {
                conditions.Add("Name LIKE @CategoryName");
                parameters.Add("CategoryName", $"%{query.CategoryName}%");
            }

            if (query.Level.HasValue)
            {
                conditions.Add("Level = @Level");
                parameters.Add("Level", query.Level.Value);
            }

            string whereConditions = conditions.Count > 0
                ? "WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            string sql = $@"
                -- Get total count for pagination        
                SELECT COUNT(1) 
                FROM categories.Categories
                {whereConditions};

                -- Get paginated results        
                SELECT Id, Name, Description, Slug, Level, DisplayOrder, ParentCategoryId, CreatedAt
                FROM categories.Categories
                {whereConditions}
                ORDER BY Level, DisplayOrder
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            int offset = (query.PageNumber - 1) * query.PageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, parameters, cancellationToken: ct);

            using var multi = await connection.QueryMultipleAsync(command);
            var totalCount = await multi.ReadFirstAsync<int>();
            var categories = (await multi.ReadAsync<CategoryResponse>()).ToList();

            return (totalCount, categories);
        }

    }
}
