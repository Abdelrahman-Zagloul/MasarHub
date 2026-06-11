using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.DeleteLesson
{
    public sealed record DeleteLessonCommand
    (
        Guid CourseId,
        Guid ModuleId,
        Guid LessonId,
        Guid InstructorId
    ) : IRequest<Result>;
}
