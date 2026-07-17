using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Courses.Events
{
    public sealed record CourseAnnouncementScheduledDomainEvent
    (
        Guid AnnouncementId,
        Guid CourseId,
        DateTimeOffset ScheduledAt
    ) : DomainEvent;
}
