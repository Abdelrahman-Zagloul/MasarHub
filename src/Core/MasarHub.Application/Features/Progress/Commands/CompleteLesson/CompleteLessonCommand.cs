using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Progress.Commands.CompleteLesson
{
    public sealed record CompleteLessonCommand
    (
        Guid UserId,
        Guid CourseId,
        Guid LessonId
    ) : IRequest<Result>;
}
