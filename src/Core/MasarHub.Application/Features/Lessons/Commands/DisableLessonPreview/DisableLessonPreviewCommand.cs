using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.DisableLessonPreview
{
    public sealed record DisableLessonPreviewCommand
    (
        Guid ModuleId,
        Guid LessonId,
        Guid InstructorId
    ) : IRequest<Result>;
}
