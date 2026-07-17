using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Features.Announcements.Queries.GetStudentCourseAnnouncements
{
    public sealed record GetStudentCourseAnnouncementsRequest
    (
        AnnouncementImportance? Importance,
        bool? IsPinned,
        string? Search,
        int PageNumber = 1,
        int PageSize = 10
    );
}
