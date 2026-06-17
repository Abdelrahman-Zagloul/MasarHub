using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Exams;
using MediatR;

namespace MasarHub.Application.Features.Questions.Commands.CreateQuestion
{
    public sealed record CreateQuestionCommand
    (
        Guid ExamId,
        Guid InstructorId,
        string QuestionText,
        decimal QuestionMark,
        QuestionType QuestionType,
        IEnumerable<Question.OptionInput> Options
    ) : IRequest<Result<CreateQuestionResponse>>;
}
