using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Questions.Queries.GetAllQuestionsByExamId;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class QuestionQuery : IQuestionQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public QuestionQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<QuestionQueryResult?> GetQuestionByIdAsync(Guid questionId, Guid examId, Guid instructorId, CancellationToken ct = default)
        {
            const string sql = @"
                SELECT
                    q.Id,
                    q.ExamId,
                    q.QuestionText,
                    q.QuestionMark,
                    q.QuestionType
                FROM exams.Questions q
                INNER JOIN exams.Exams e ON e.Id = q.ExamId AND e.IsDeleted = 0
                INNER JOIN courses.Courses c ON c.Id = e.CourseId AND c.IsDeleted = 0
                WHERE q.Id = @QuestionId
                  AND q.ExamId = @ExamId
                  AND c.InstructorId = @InstructorId;

                SELECT
                    o.Id,
                    o.QuestionId,
                    o.Text,
                    o.IsCorrect
                FROM exams.Options o
                WHERE o.QuestionId = @QuestionId
                ORDER BY o.CreatedAt;";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { questionId, examId, instructorId }, cancellationToken: ct);
            using var multi = await connection.QueryMultipleAsync(command);

            var question = await multi.ReadFirstOrDefaultAsync<QuestionQueryResult>();
            if (question is null)
                return null;

            var options = (await multi.ReadAsync<OptionQueryResult>()).ToList();
            return question with { Options = options };
        }

        public async Task<IReadOnlyList<QuestionQueryResult>> GetAllQuestionsByExamIdAsync(GetAllQuestionsByExamIdQuery query, CancellationToken ct = default)
        {
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            parameters.Add("ExamId", query.ExamId);
            parameters.Add("InstructorId", query.InstructorId);
            if (query.QuestionType.HasValue)
            {
                conditions.Add("QuestionType = @QuestionType");
                parameters.Add("QuestionType", query.QuestionType.Value.ToString());
            }

            var questionTypeFilter = conditions.Count > 0
            ? $" AND {string.Join(" AND ", conditions)}"
            : string.Empty;

            var sql = $@"
                SELECT
                    q.Id,
                    q.ExamId,
                    q.QuestionText,
                    q.QuestionMark,
                    q.QuestionType
                FROM exams.Questions q
                INNER JOIN exams.Exams e ON e.Id = q.ExamId AND e.IsDeleted = 0
                INNER JOIN courses.Courses c ON c.Id = e.CourseId AND c.IsDeleted = 0
                WHERE q.ExamId = @ExamId
                  AND c.InstructorId = @InstructorId {questionTypeFilter}
                ORDER BY q.CreatedAt;

                SELECT
                    o.Id,
                    o.QuestionId,
                    o.Text,
                    o.IsCorrect
                FROM exams.Options o
                INNER JOIN exams.Questions q ON q.Id = o.QuestionId
                WHERE q.ExamId = @ExamId {questionTypeFilter}
                ORDER BY o.CreatedAt;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, parameters, cancellationToken: ct);

            using var multi = await connection.QueryMultipleAsync(command);
            var questions = (await multi.ReadAsync<QuestionQueryResult>()).ToList();

            if (questions.Count == 0)
                return questions;

            var options = await multi.ReadAsync<OptionQueryResult>();
            var optionsByQuestion = options.ToLookup(o => o.QuestionId);

            for (int i = 0; i < questions.Count; i++)
                questions[i] = questions[i] with { Options = optionsByQuestion[questions[i].Id].ToList() };

            return questions;
        }
    }
}
