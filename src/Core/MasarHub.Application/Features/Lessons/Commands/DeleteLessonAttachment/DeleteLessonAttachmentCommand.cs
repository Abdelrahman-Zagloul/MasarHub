using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.DeleteLessonAttachment
{
    public sealed record DeleteLessonAttachmentCommand
    (
        Guid LessonId,
        Guid AttachmentId,
        Guid InstructorId
    ) : IRequest<Result>;
}
