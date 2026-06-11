using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.ReorderLessons
{
    public sealed record ReorderLessonsCommand
    (
        Guid ModuleId,
        Guid InstructorId,
        IReadOnlyList<Guid> OrderedLessonIds
    ) : IRequest<Result>;

}
