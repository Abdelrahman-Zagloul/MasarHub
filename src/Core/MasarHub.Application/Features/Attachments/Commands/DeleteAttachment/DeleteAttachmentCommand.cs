using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Attachments.Commands.DeleteAttachment
{
    public sealed record DeleteAttachmentCommand
    (
        Guid LessonId,
        Guid AttachmentId,
        Guid InstructorId
    ) : IRequest<Result>;
}
