using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Courses.Events
{
    public sealed record CourseRejectedDomainEvent
    (
         Guid CourseId,
         Guid InstructorId,
         string CourseTitle,
         string RejectionReason
    ) : DomainEvent;
}
