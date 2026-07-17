using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent
{
    public sealed record StudentCourseAnnouncementResponse
    (
        Guid Id,
        Guid CourseId,
        string Title,
        string Content,
        DateTimeOffset? PublishedAt,
        AnnouncementImportance Importance,
        bool IsPinned,
        DateTimeOffset CreatedAt
    );
}
