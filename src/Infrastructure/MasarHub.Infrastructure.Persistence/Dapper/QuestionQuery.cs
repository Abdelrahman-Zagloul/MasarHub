using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;

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
    }
}
