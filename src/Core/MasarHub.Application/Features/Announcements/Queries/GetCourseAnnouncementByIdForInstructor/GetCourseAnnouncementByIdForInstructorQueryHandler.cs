using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor
{
    public sealed class GetCourseAnnouncementByIdForInstructorQueryHandler : IRequestHandler<GetCourseAnnouncementByIdForInstructorQuery, Result<InstructorCourseAnnouncementResponse>>
    {
        private readonly IAnnouncementQuery _announcementQuery;

        public GetCourseAnnouncementByIdForInstructorQueryHandler(IAnnouncementQuery announcementQuery)
        {
            _announcementQuery = announcementQuery;
        }

        public async Task<Result<InstructorCourseAnnouncementResponse>> Handle(GetCourseAnnouncementByIdForInstructorQuery request, CancellationToken cancellationToken)
        {
            var announcement = await _announcementQuery.GetByIdForInstructorAsync(request.CourseId, request.AnnouncementId, request.InstructorId, cancellationToken);
            if (announcement is null)
                return Error.NotFound("course_announcement.not_found");

            return announcement;
        }
    }
}
