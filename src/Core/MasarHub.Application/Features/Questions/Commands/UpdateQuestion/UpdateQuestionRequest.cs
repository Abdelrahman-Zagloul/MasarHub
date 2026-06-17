namespace MasarHub.Application.Features.Questions.Commands.UpdateQuestion
{
    public sealed record UpdateQuestionRequest
    (
        string? QuestionText,
        decimal? QuestionMark,
        List<OptionUpdateRequest>? Options
    );

    public sealed record OptionUpdateRequest(Guid OptionId, string? Text, bool? IsCorrect);
}
