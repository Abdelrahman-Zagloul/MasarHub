using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Courses.Events
{
    public sealed record CourseReviewCreatedDomainEvent
    (
        Guid ReviewId,
        Guid UserId,
        Guid CourseId,
        double Rating,
        string? ReviewContent
    ) : DomainEvent;
}
