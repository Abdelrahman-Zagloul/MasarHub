using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Queries.GetStudentCourseAnnouncements
{
    public sealed class GetStudentCourseAnnouncementsQueryHandler : IRequestHandler<GetStudentCourseAnnouncementsQuery, Result<PaginatedResult<StudentCourseAnnouncementResponse>>>
    {
        private readonly IAnnouncementQuery _announcementQuery;

        public GetStudentCourseAnnouncementsQueryHandler(IAnnouncementQuery announcementQuery)
        {
            _announcementQuery = announcementQuery;
        }

        public async Task<Result<PaginatedResult<StudentCourseAnnouncementResponse>>> Handle(GetStudentCourseAnnouncementsQuery request, CancellationToken cancellationToken)
        {
            var result = await _announcementQuery.GetStudentListAsync(request, cancellationToken);
            if (!result.IsEnrolled)
                return Error.Forbidden("course.not_enrolled");

            return PaginatedResult<StudentCourseAnnouncementResponse>.Create(
                result.Announcement?.Items ?? [],
                result.Announcement?.TotalCount ?? 0,
                request.PageNumber,
                request.PageSize
            );
        }
    }
}
