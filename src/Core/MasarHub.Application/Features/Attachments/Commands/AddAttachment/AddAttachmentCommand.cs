using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Attachments.Commands.AddAttachment
{
    public sealed record AddAttachmentCommand
    (
        Guid LessonId,
        Guid InstructorId,
        FileResource File
    ) : IRequest<Result<AddAttachmentResponse>>;
}
