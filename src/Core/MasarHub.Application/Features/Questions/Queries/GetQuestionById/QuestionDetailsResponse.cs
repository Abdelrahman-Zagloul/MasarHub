namespace MasarHub.Application.Features.Questions.Queries.GetQuestionById
{
    public sealed record QuestionDetailsResponse
    (
        Guid Id,
        Guid ExamId,
        string QuestionText,
        decimal QuestionMark,
        string QuestionType,
        IReadOnlyList<OptionResponse> Options
    );

    public sealed record OptionResponse(Guid Id, string Text, bool IsCorrect);
}
