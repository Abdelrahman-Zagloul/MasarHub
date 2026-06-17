using MasarHub.Domain.Modules.Exams;

namespace MasarHub.Application.Features.Questions.Commands.CreateQuestion
{
    public sealed record CreateQuestionResponse
    (
        Guid Id,
        Guid ExamId,
        string QuestionText,
        decimal QuestionMark,
        QuestionType QuestionType,
        List<OptionResponse> Options
    );

    public sealed record OptionResponse(Guid Id, string Text, bool IsCorrect);
}
