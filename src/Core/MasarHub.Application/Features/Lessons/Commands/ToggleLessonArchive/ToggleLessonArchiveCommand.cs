using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.ToggleLessonArchive
{
    public sealed record ToggleLessonArchiveCommand
    (
        Guid ModuleId,
        Guid LessonId,
        Guid InstructorId,
        bool Archived
    ) : IRequest<Result>;
}
