using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent
{
    public sealed class GetCourseAnnouncementByIdForStudentQueryHandler : IRequestHandler<GetCourseAnnouncementByIdForStudentQuery, Result<StudentCourseAnnouncementResponse>>
    {
        private readonly IAnnouncementQuery _announcementQuery;

        public GetCourseAnnouncementByIdForStudentQueryHandler(IAnnouncementQuery announcementQuery)
        {
            _announcementQuery = announcementQuery;
        }

        public async Task<Result<StudentCourseAnnouncementResponse>> Handle(GetCourseAnnouncementByIdForStudentQuery request, CancellationToken cancellationToken)
        {
            var result = await _announcementQuery.GetByIdForStudentAsync(request.CourseId, request.AnnouncementId, request.StudentId, cancellationToken);

            if (!result.IsEnrolled)
                return Error.Forbidden("course.not_enrolled");

            if (result.Announcement == null)
                return Error.NotFound("course_announcement.not_found");

            return result.Announcement;
        }
    }
}
