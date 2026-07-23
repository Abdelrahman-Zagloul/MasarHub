using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Exams.Commands.StartExamAttempt
{
    public sealed record StartExamAttemptCommand
    (
        Guid CourseId,
        Guid ExamId,
        Guid UserId
    ) : IRequest<Result<StartExamAttemptResponse>>;
}
