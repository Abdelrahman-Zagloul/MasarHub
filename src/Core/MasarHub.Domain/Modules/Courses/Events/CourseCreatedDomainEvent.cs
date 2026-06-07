using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Courses.Events
{
    public sealed record CourseCreatedDomainEvent
    (
        Guid CourseId,
        Guid InstructorId,
        string CourseTitle
    ) : DomainEvent;
}
