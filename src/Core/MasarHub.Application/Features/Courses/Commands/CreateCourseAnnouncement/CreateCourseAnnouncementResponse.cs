using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Features.Courses.Commands.CreateCourseAnnouncement
{
    public sealed record CreateCourseAnnouncementResponse
    (
        Guid Id,
        Guid CourseId,
        string Title,
        string Content,
        AnnouncementImportance Importance,
        DateTimeOffset CreatedAt
    );
}
