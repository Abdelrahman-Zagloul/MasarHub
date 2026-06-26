using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Courses.Events
{
    public sealed record CourseEnrollmentCreatedDomainEvent
    (
        Guid EnrollmentId,
        Guid UserId,
        Guid CourseId,
        string CourseTitle,
        Guid OrderId,
        decimal PaidAmount
    ) : DomainEvent;
}
