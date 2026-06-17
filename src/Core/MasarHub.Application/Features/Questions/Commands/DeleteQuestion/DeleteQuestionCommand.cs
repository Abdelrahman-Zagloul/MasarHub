using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Questions.Commands.DeleteQuestion
{
    public sealed record DeleteQuestionCommand
    (
        Guid ExamId,
        Guid QuestionId,
        Guid InstructorId
    ) : IRequest<Result>;
}
