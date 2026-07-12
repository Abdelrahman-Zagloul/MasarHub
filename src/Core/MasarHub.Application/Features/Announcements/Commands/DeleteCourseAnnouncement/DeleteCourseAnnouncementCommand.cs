using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Commands.DeleteCourseAnnouncement
{
    public sealed record DeleteCourseAnnouncementCommand
    (
        Guid CourseId,
        Guid AnnouncementId,
        Guid InstructorId
    ) : IRequest<Result>;
}
