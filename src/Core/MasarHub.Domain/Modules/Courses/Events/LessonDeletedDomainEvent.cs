using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Courses.Events
{
    public sealed record LessonDeletedDomainEvent(Guid ModuleId, Guid LessonId) : DomainEvent;
}
