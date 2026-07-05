using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Courses.Events
{
    public sealed record LessonProgressCreatedDomainEvent
    (
        Guid UserId,
        Guid CourseId,
        Guid ModuleId,
        Guid LessonId
    ) : DomainEvent;
}
