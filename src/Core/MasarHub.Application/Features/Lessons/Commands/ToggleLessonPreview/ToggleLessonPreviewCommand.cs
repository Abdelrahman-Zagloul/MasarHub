using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.ToggleLessonPreview
{
    public sealed record ToggleLessonPreviewCommand
    (
        Guid ModuleId,
        Guid LessonId,
        Guid InstructorId,
        bool Previewable
    ) : IRequest<Result>;
}
