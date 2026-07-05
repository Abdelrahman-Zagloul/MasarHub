using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Progress.Queries.GetCourseProgress;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class ProgressQuery : IProgressQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProgressQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<CreateProgressData> GetCreateProgressDataAsync(Guid userId, Guid courseId, Guid lessonId, CancellationToken ct)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT ModuleId FROM courses.Lessons WHERE Id = @LessonId AND IsDeleted = 0;

                SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM courses.CourseEnrollments
                    WHERE UserId = @UserId AND CourseId = @CourseId AND IsDeleted = 0
                ) THEN 1 ELSE 0 END AS BIT);

                SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM courses.LessonProgress
                    WHERE UserId = @UserId AND LessonId = @LessonId AND IsDeleted = 0
                ) THEN 1 ELSE 0 END AS BIT);";

            using var multi = await connection.QueryMultipleAsync(sql, new { UserId = userId, CourseId = courseId, LessonId = lessonId });

            var moduleId = await multi.ReadFirstOrDefaultAsync<Guid?>();
            var hasEnrollment = await multi.ReadSingleAsync<bool>();
            var alreadyCompleted = await multi.ReadSingleAsync<bool>();

            return new CreateProgressData(hasEnrollment, moduleId != null, alreadyCompleted, moduleId);
        }

        public async Task<int> GetModuleLessonCountAsync(Guid moduleId, CancellationToken ct)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT COUNT(*) FROM courses.Lessons
                WHERE ModuleId = @ModuleId AND IsDeleted = 0";

            return await connection.QuerySingleAsync<int>(sql, new { ModuleId = moduleId });
        }

        public async Task<int> GetCourseLessonCountAsync(Guid courseId, CancellationToken ct)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT COUNT(*)
                FROM courses.Lessons l
                INNER JOIN courses.CourseModules m ON m.Id = l.ModuleId
                WHERE m.CourseId = @CourseId AND l.IsDeleted = 0 AND m.IsDeleted = 0";

            return await connection.QuerySingleAsync<int>(sql, new { CourseId = courseId });
        }

        public async Task<CourseProgressData> GetCourseProgressAsync(Guid userId, Guid courseId, CancellationToken ct)
        {
            const string sql = @"
                SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM courses.CourseEnrollments
                    WHERE UserId = @UserId AND CourseId = @CourseId AND IsDeleted = 0
                ) THEN 1 ELSE 0 END AS BIT) AS HasEnrollment;

                SELECT CompletedLessons, TotalLessons, CompletedAt
                FROM courses.CourseProgress
                WHERE UserId = @UserId AND CourseId = @CourseId AND IsDeleted = 0;

                SELECT mp.ModuleId, m.Title AS ModuleTitle, m.DisplayOrder, mp.CompletedLessons, mp.TotalLessons
                FROM courses.ModuleProgress mp
                INNER JOIN courses.CourseModules m ON m.Id = mp.ModuleId
                WHERE mp.UserId = @UserId AND mp.CourseId = @CourseId AND mp.IsDeleted = 0
                ORDER BY m.DisplayOrder;";


            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { UserId = userId, CourseId = courseId }, cancellationToken: ct);

            using var multi = await connection.QueryMultipleAsync(command);
            var hasEnrollment = await multi.ReadSingleAsync<bool>();
            if (!hasEnrollment)
                return new CourseProgressData(false, null, []);

            var courseRow = await multi.ReadFirstOrDefaultAsync<CourseProgress>();
            if (courseRow == null)
                return new CourseProgressData(true, null, []);

            var modules = (await multi.ReadAsync<ModuleProgressItem>()).ToList();
            return new CourseProgressData(hasEnrollment, courseRow, modules);
        }
    }
}
