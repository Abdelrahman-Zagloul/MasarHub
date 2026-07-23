using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Domain.Modules.Exams;

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
                    CAST(CASE WHEN c.InstructorId = @InstructorId THEN 1 ELSE 0 END AS BIT) AS IsOwner,
                    e.IsPublished
                FROM exams.Exams e
                INNER JOIN courses.Courses c ON c.Id = e.CourseId AND c.IsDeleted = 0
                WHERE e.Id = @ExamId AND e.IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();

            var command = new CommandDefinition(sql, new { examId, instructorId }, cancellationToken: ct);
            return await connection.QuerySingleOrDefaultAsync<ExamUpdateData>(command) ?? new ExamUpdateData(false, false, false);
        }

        public async Task<ExamState> GetExamStateAsync(Guid examId, Guid instructorId, CancellationToken ct = default)
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
            var result = await connection.QuerySingleOrDefaultAsync<ExamState>(command);
            return result ?? new ExamState(false, false, false);
        }

        public async Task<Exam?> GetExamDetailsAsync(Guid examId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT Id, CourseId, ModuleId, Title, Description, PassingScorePercentage, DurationInMinutes, MaxAttempts, IsPublished, CreatedAt, UpdatedAt, IsDeleted, DeletedAt
                FROM exams.Exams WHERE Id = @ExamId AND IsDeleted = 0;
            
                SELECT Id, ExamId, QuestionText, QuestionMark, QuestionType, CreatedAt, UpdatedAt   
                FROM exams.Questions WHERE ExamId = @ExamId;
                
                SELECT o.Id, o.QuestionId, o.Text, o.IsCorrect, o.CreatedAt, o.UpdatedAt 
                FROM exams.Options o INNER JOIN exams.Questions q ON q.Id = o.QuestionId WHERE q.ExamId = @ExamId;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { ExamId = examId }, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var exam = await multi.ReadSingleOrDefaultAsync<Exam>();
            if (exam is null)
                return null;

            var questions = (await multi.ReadAsync<Question>()).ToList();

            var optionsByQuestion = (await multi.ReadAsync<Option>())
                .GroupBy(o => o.QuestionId)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());

            foreach (var question in questions)
                if (optionsByQuestion.TryGetValue(question.Id, out var options))
                    question.LoadOptions(options);

            exam.LoadQuestions(questions);
            return exam;
        }

        public async Task<ExamAttemptStartData?> GetAttemptStartDataAsync(Guid examId, Guid courseId, Guid userId, CancellationToken ct = default)
        {
            const string sql = @"
                 SELECT CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM courses.CourseEnrollments
                    WHERE CourseId = @CourseId AND UserId = @userId AND IsDeleted = 0 AND Status = 'Active'
                ) THEN 1 ELSE 0 END AS BIT);

                SELECT
                    COUNT(CASE WHEN Status IN ('Submitted', 'Cancelled') THEN 1 END) AS CompletedCount,
                    MAX(CASE WHEN Status = 'InProgress' THEN Id END) AS InProgressId
                FROM exams.ExamAttempts WHERE ExamId = @ExamId AND UserId = @UserId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { examId, courseId, userId }, cancellationToken: ct);

            using var multi = await connection.QueryMultipleAsync(command);
            var isEnrolled = await multi.ReadSingleOrDefaultAsync<bool>();
            var (completedCount, inProgressId) = await multi.ReadSingleOrDefaultAsync<(int, Guid?)>();
            return new ExamAttemptStartData(isEnrolled, completedCount, inProgressId);
        }
    }
}
