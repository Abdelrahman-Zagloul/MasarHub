using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent
{
    public sealed record GetCourseAnnouncementByIdForStudentQuery
    (
        Guid CourseId,
        Guid AnnouncementId,
        Guid StudentId
    ) : IRequest<Result<StudentCourseAnnouncementResponse>>;
}
