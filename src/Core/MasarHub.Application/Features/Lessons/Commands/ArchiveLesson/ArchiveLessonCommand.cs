using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.ArchiveLesson
{
    public sealed record ArchiveLessonCommand
    (
        Guid CourseId,
        Guid ModuleId,
        Guid LessonId,
        Guid InstructorId
    ) : IRequest<Result>;
}
