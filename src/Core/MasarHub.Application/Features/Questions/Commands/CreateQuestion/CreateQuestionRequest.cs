using MasarHub.Domain.Modules.Exams;

namespace MasarHub.Application.Features.Questions.Commands.CreateQuestion
{
    public sealed record CreateQuestionRequest
    (
        string QuestionText,
        decimal QuestionMark,
        QuestionType QuestionType,
        List<CreateOptionRequest> Options
    );

    public sealed record CreateOptionRequest(string Text, bool IsCorrect);
}
