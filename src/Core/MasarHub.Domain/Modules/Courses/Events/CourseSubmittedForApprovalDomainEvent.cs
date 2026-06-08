using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Courses.Events
{
    public sealed record CourseSubmittedForApprovalDomainEvent
    (
        Guid CourseId,
        Guid InstructorId
    ) : DomainEvent;
}
