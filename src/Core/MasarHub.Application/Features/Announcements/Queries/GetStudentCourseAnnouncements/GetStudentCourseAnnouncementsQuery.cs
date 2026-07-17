using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Queries.GetStudentCourseAnnouncements
{
    public sealed record GetStudentCourseAnnouncementsQuery
    (
        Guid CourseId,
        Guid StudentId,
        AnnouncementImportance? Importance,
        bool? IsPinned,
        string? Search,
        int PageNumber = 1,
        int PageSize = 10
    ) : IPaginatedQuery, IRequest<Result<PaginatedResult<StudentCourseAnnouncementResponse>>>;
}
