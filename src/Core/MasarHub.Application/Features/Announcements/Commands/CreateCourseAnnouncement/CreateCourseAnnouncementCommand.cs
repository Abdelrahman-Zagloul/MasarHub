using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Commands.CreateCourseAnnouncement
{
    public sealed record CreateCourseAnnouncementCommand
    (
        Guid CourseId,
        Guid InstructorId,
        string Title,
        string Content,
        AnnouncementImportance Importance
    ) : IRequest<Result<CreateCourseAnnouncementResponse>>;
}
