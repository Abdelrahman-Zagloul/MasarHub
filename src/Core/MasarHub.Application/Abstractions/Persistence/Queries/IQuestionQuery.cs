using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface IQuestionQuery : IScopedService
    {
        Task<QuestionQueryResult?> GetQuestionByIdAsync(Guid questionId, Guid examId, Guid instructorId, CancellationToken ct = default);
    }

    public sealed record QuestionQueryResult(Guid Id, Guid ExamId, string QuestionText, decimal QuestionMark, string QuestionType)
    {
        public IReadOnlyList<OptionQueryResult> Options { get; init; } = [];
    }
    public sealed record OptionQueryResult(Guid Id, string Text, bool IsCorrect);

}
