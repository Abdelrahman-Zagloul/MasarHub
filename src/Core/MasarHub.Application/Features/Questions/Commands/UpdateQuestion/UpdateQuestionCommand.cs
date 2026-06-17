using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Exams;
using MediatR;

namespace MasarHub.Application.Features.Questions.Commands.UpdateQuestion
{
    public sealed record UpdateQuestionCommand
    (
        Guid ExamId,
        Guid QuestionId,
        Guid InstructorId,
        string? QuestionText,
        decimal? QuestionMark,
        List<Question.OptionUpdateInput>? Options
    ) : IRequest<Result>;
}
