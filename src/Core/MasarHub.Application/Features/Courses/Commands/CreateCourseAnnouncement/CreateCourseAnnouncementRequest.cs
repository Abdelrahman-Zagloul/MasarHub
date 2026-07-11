using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Features.Courses.Commands.CreateCourseAnnouncement
{
    public sealed record CreateCourseAnnouncementRequest
    (
        string Title,
        string Content,
        AnnouncementImportance Importance
    );
}
