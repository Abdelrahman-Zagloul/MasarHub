using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Commands.PublishCourseAnnouncement
{
    public sealed record PublishCourseAnnouncementCommand
    (
        Guid CourseId,
        Guid AnnouncementId,
        Guid InstructorId
    ) : IRequest<Result>;
}
