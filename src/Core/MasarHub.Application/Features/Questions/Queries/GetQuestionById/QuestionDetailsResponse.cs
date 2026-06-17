namespace MasarHub.Application.Features.Questions.Queries.GetQuestionById
{
    public sealed record QuestionDetailsResponse
    (
        Guid QuestionId,
        Guid ExamId,
        string QuestionText,
        decimal QuestionMark,
        string QuestionType,
        IReadOnlyList<OptionResponse> Options
    );

    public sealed record OptionResponse(Guid OptionId, Guid QuestionId, string Text, bool IsCorrect);
}
