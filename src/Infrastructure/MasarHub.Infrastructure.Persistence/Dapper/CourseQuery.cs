using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Courses.Queries.GetCourseById;
using MasarHub.Application.Features.Courses.Queries.GetCourses;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class CourseQuery : ICourseQuery
    {

        private readonly IDbConnectionFactory _connectionFactory;

        public CourseQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<CourseCreationData> GetCreationDataAsync(string slug, Guid categoryId, CancellationToken ct)
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

            return new CourseCreationData(await multi.ReadFirstAsync<bool>(), await multi.ReadFirstAsync<int>());
        }
        public async Task<UserInfo?> GetUserInfoAsync(Guid instructorId, CancellationToken ct)
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

            return await connection.QueryFirstOrDefaultAsync<UserInfo>(command);
        }
        public async Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct)
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
        public async Task<bool> HasLecturesAsync(Guid courseId, CancellationToken ct)
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
            var command = new CommandDefinition(sql, new { CourseId = courseId }, cancellationToken: ct);
            return await connection.ExecuteScalarAsync<bool>(command);
        }
        public async Task<CourseDetailsResponse?> GetDetailsByIdAsync(Guid courseId, CancellationToken ct)
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
            var command = new CommandDefinition(sql, new { CourseId = courseId }, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var course = await multi.ReadFirstOrDefaultAsync<CourseDetailsResponse>();
            if (course is null)
                return null;

            course.Prerequisites = (await multi.ReadAsync<string>()).ToList();
            course.Requirements = (await multi.ReadAsync<string>()).ToList();
            course.LearningObjectives = (await multi.ReadAsync<string>()).ToList();
            return course;
        }
        public async Task<CourseThumbnailDetails> GetThumbnailDetailsAsync(Guid courseId, CancellationToken ct)
        {
            var sql = @"
                SELECT 
                    CAST(1 AS BIT) AS CourseExists,
                    ThumbnailPublicId
                FROM courses.Courses
                WHERE Id = @CourseId AND IsDeleted = 0;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { CourseId = courseId }, cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<CourseThumbnailDetails>(command);
            return result ?? new CourseThumbnailDetails(false, null);
        }
        public async Task<PagedResult<CourseResponse>> GetAllAsync(GetCoursesQuery query, CourseStatus? status, CancellationToken ct)
        {
            var conditions = new List<string> { "c.IsDeleted = 0", };
            var parameters = new DynamicParameters();

            if (status.HasValue)
            {
                parameters.Add("Status", status.ToString());
                conditions.Add("c.Status = @Status");
            }

            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                conditions.Add("c.Title LIKE @Title");
                parameters.Add("Title", $"%{query.Title}%");
            }

            if (query.CategoryId.HasValue)
            {
                conditions.Add("c.CategoryId = @CategoryId");
                parameters.Add("CategoryId", query.CategoryId.Value);
            }

            if (query.InstructorId.HasValue)
            {
                conditions.Add("c.InstructorId = @InstructorId");
                parameters.Add("InstructorId", query.InstructorId.Value);
            }

            if (query.Language.HasValue)
            {
                conditions.Add("c.Language = @Language");
                parameters.Add("Language", query.Language.Value.ToString());
            }

            if (query.Level.HasValue)
            {
                conditions.Add("c.Level = @Level");
                parameters.Add("Level", query.Level.Value.ToString());
            }

            if (query.MinPrice.HasValue)
            {
                conditions.Add("c.Price >= @MinPrice");
                parameters.Add("MinPrice", query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                conditions.Add("c.Price <= @MaxPrice");
                parameters.Add("MaxPrice", query.MaxPrice.Value);
            }

            string whereConditions = conditions.Count > 0
                ? "WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            string sql = $@"
                -- Get total count for pagination
                SELECT COUNT(1) 
                FROM courses.Courses c 
                {whereConditions};

                -- Get paginated results
                SELECT 
                    c.Id, 
                    c.Title, 
                    c.Slug, 
                    c.Price, 
                    c.Language, 
                    c.Status, 
                    c.Level, 
                    c.PublishedAt,
                    c.InstructorId,
                    u.FullName AS InstructorName, 
                    c.CategoryId,
                    cat.Name AS CategoryName
                FROM courses.Courses c
                LEFT JOIN [identity].Users u ON c.InstructorId = u.Id
                LEFT JOIN categories.Categories cat ON c.CategoryId = cat.Id
                {whereConditions}
                ORDER BY c.PublishedAt DESC, c.Id
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            int offset = (query.PageNumber - 1) * query.PageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
            var multi = await connection.QueryMultipleAsync(command);

            var totalCount = await multi.ReadFirstAsync<int>();
            var courses = (await multi.ReadAsync<CourseResponse>()).ToList();
            return new PagedResult<CourseResponse>(courses, totalCount);
        }
        public async Task<CourseAccessData> GetCourseAccessData(Guid courseId, Guid instructorId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT
                    CAST(
                        CASE
                            WHEN EXISTS (
                                SELECT 1
                                FROM courses.Courses
                                WHERE Id = @CourseId AND IsDeleted = 0
                            )
                            THEN 1
                            ELSE 0
                        END AS BIT
                    ) AS CourseExist,

                    CAST(
                        CASE
                            WHEN EXISTS (
                                SELECT 1
                                FROM courses.Courses
                                WHERE Id = @CourseId
                                  AND InstructorId = @InstructorId AND IsDeleted = 0
                            )
                            THEN 1
                            ELSE 0
                        END AS BIT
                    ) AS IsOwner;
                ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql,
                new
                {
                    CourseId = courseId,
                    InstructorId = instructorId
                },
                cancellationToken: ct);

            return await connection.QuerySingleAsync<CourseAccessData>(command);
        }
        public async Task<CourseCartData?> GetCourseCartDataAsync(Guid courseId, CancellationToken ct)
        {
            const string sql = @"
                SELECT
                    Id,
                    Title,
                    Price,
                    ThumbnailPublicId,
                    CAST(CASE WHEN Status = 'Published' THEN 1 ELSE 0 END AS BIT) AS IsPublished
                FROM courses.Courses
                WHERE Id = @CourseId AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { CourseId = courseId }, cancellationToken: ct);
            return await connection.QuerySingleOrDefaultAsync<CourseCartData>(command);
        }
    }
}
