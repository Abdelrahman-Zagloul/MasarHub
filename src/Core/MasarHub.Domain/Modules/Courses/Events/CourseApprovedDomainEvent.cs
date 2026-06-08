using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Courses.Events
{
    public sealed record CourseApprovedDomainEvent
    (
         Guid CourseId,
         Guid InstructorId,
         string CourseTitle
    ) : DomainEvent;
}
