using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor
{
    public sealed record GetCourseAnnouncementByIdForInstructorQuery
    (
        Guid CourseId,
        Guid AnnouncementId,
        Guid InstructorId
    ) : IRequest<Result<InstructorCourseAnnouncementResponse>>;
}
