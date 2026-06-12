using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
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
        public async Task<CategoryCreationData> GetCreationDataAsync(string slug, Guid? parentCategoryId, CancellationToken ct)
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
            int nextDisplayOrder = await multi.ReadFirstAsync<int>();
            bool slugExists = await multi.ReadFirstAsync<bool>();
            return new CategoryCreationData(nextDisplayOrder, slugExists);
        }
        public async Task<CategoryDeletionCheckData> CanDeleteAsync(Guid id, CancellationToken ct = default)
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
            bool hasChildren = await multi.ReadFirstAsync<bool>();
            bool hasCourses = await multi.ReadFirstAsync<bool>();
            return new CategoryDeletionCheckData(hasChildren, hasCourses);
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
        public async Task<PagedResult<CategoryResponse>> GetAllAsync(GetCategoriesQuery query, CancellationToken ct = default)
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

            return new PagedResult<CategoryResponse>(categories, totalCount);
        }
        public async Task<List<Guid>> GetCategoryIdsByParentIdAsync(Guid? parentCategoryId, CancellationToken ct = default)
        {
            var sql = parentCategoryId is null
                ? "SELECT Id FROM categories.Categories WHERE ParentCategoryId IS NULL"
                : "SELECT Id FROM categories.Categories WHERE ParentCategoryId = @ParentCategoryId";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { ParentCategoryId = parentCategoryId }, cancellationToken: ct);

            var ids = await connection.QueryAsync<Guid>(command);
            return ids.ToList();
        }
        public async Task<bool> BulkUpdateDisplayOrderAsync(Guid? parentCategoryId, IReadOnlyList<Guid> orderedCategoryIds, CancellationToken ct = default)
        {
            var valuesList = orderedCategoryIds.Select((id, index) => $"('{id}', {index + 1})");
            var valuesRows = string.Join(", ", valuesList);

            var parentCategoryCondition = parentCategoryId is null
                ? "C.ParentCategoryId IS NULL"
                : "C.ParentCategoryId = @ParentCategoryId";

            var sql = $@"
                UPDATE C
                SET C.DisplayOrder = T.NewOrder,
                    C.UpdatedAt = SYSUTCDATETIME()
                FROM categories.Categories C
                INNER JOIN (
                    VALUES {valuesRows}
                ) AS T(CategoryId, NewOrder) ON C.Id = CAST(T.CategoryId AS uniqueidentifier)
                WHERE {parentCategoryCondition};";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { ParentCategoryId = parentCategoryId }, cancellationToken: ct);

            var affectedRows = await connection.ExecuteAsync(command);
            return affectedRows > 0;
        }
    }
}
