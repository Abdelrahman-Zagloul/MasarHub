using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Commands.UpdateCourseAnnouncement
{
    public sealed record UpdateCourseAnnouncementCommand
    (
        Guid CourseId,
        Guid AnnouncementId,
        Guid InstructorId,
        string? Title,
        string? Content,
        AnnouncementImportance? Importance
    ) : IRequest<Result>;
}
