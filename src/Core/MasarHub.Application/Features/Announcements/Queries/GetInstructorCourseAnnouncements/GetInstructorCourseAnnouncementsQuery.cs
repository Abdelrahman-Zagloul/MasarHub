using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Queries.GetInstructorCourseAnnouncements
{
    public sealed record GetInstructorCourseAnnouncementsQuery
    (
        Guid CourseId,
        Guid InstructorId,
        AnnouncementImportance? Importance,
        bool? IsPublished,
        bool? IsPinned,
        string? Search,
        int PageNumber,
        int PageSize
    ) : IPaginatedQuery, IRequest<Result<PaginatedResult<InstructorCourseAnnouncementResponse>>>;
}
