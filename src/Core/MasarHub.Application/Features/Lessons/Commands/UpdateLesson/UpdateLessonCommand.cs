using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.UpdateLesson
{
    public sealed record UpdateLessonCommand
    (
        Guid ModuleId,
        Guid LessonId,
        Guid InstructorId,
        string? Title,
        string? Description
    ) : IRequest<Result>;
}
