using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.EnableLessonPreview
{
    public sealed record EnableLessonPreviewCommand
    (
        Guid CourseId,
        Guid ModuleId,
        Guid LessonId,
        Guid InstructorId
    ) : IRequest<Result>;
    public sealed record SetLessonPreviewRequest(bool IsPreviewable);
}
