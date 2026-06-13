using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.UpdateVideoThumbnail
{
    public sealed record UpdateVideoThumbnailCommand
    (
        Guid ModuleId,
        Guid LessonId,
        Guid InstructorId,
        FileResource File
    ) : IRequest<Result>;
}
