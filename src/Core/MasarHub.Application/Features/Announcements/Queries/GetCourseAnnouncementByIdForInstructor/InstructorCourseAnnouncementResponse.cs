using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor
{
    public sealed record InstructorCourseAnnouncementResponse
    (
        Guid Id,
        Guid CourseId,
        Guid InstructorId,
        string Title,
        string Content,
        bool IsPublished,
        DateTimeOffset? PublishedAt,
        DateTimeOffset? ScheduledAt,
        DateTimeOffset? ExpiresAt,
        AnnouncementImportance Importance,
        bool IsPinned,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt
    );
}
