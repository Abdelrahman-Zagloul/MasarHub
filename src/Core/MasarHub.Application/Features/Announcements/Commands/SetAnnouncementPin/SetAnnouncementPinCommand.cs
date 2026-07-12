using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Commands.SetAnnouncementPin
{
    public sealed record SetAnnouncementPinCommand
    (
        Guid CourseId,
        Guid AnnouncementId,
        Guid InstructorId,
        bool IsPinned
    ) : IRequest<Result>;
}
