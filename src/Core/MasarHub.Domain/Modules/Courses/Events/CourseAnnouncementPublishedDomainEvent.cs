using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Courses.Events
{
    public sealed record CourseAnnouncementPublishedDomainEvent
    (
        Guid AnnouncementId,
        Guid CourseId,
        Guid InstructorId,
        string Title,
        string Content,
        AnnouncementImportance Importance,
        bool IsPinned
    ) : DomainEvent;
}
