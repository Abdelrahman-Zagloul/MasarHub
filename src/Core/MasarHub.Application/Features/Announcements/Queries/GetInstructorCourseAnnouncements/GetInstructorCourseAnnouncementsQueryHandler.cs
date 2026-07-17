using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Queries.GetInstructorCourseAnnouncements
{
    public sealed class GetInstructorCourseAnnouncementsQueryHandler : IRequestHandler<GetInstructorCourseAnnouncementsQuery, Result<PaginatedResult<InstructorCourseAnnouncementResponse>>>
    {
        private readonly IAnnouncementQuery _announcementQuery;

        public GetInstructorCourseAnnouncementsQueryHandler(IAnnouncementQuery announcementQuery)
        {
            _announcementQuery = announcementQuery;
        }

        public async Task<Result<PaginatedResult<InstructorCourseAnnouncementResponse>>> Handle(GetInstructorCourseAnnouncementsQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _announcementQuery.GetInstructorListAsync(request, cancellationToken);

            return PaginatedResult<InstructorCourseAnnouncementResponse>.Create(
                pagedResult.Items, pagedResult.TotalCount, request.PageNumber, request.PageSize);
        }
    }
}
