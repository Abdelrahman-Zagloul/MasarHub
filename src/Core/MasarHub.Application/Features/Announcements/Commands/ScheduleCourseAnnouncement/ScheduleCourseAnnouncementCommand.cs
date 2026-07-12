using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Commands.ScheduleCourseAnnouncement
{
    public sealed record ScheduleCourseAnnouncementCommand
    (
        Guid CourseId,
        Guid AnnouncementId,
        Guid InstructorId,
        DateTimeOffset ScheduledAt
    ) : IRequest<Result>;
}
