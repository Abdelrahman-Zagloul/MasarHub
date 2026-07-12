using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Features.Announcements.Commands.UpdateCourseAnnouncement
{
    public sealed record UpdateCourseAnnouncementRequest(string? Title, string? Content, AnnouncementImportance? Importance);
}
