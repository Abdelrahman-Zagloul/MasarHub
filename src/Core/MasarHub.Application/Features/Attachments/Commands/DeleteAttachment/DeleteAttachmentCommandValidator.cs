using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Attachments.Commands.DeleteAttachment
{
    public sealed class DeleteAttachmentCommandValidator : AbstractValidator<DeleteAttachmentCommand>
    {
        public DeleteAttachmentCommandValidator()
        {
            RuleFor(x => x.LessonId)
                .ValidGuid("LessonId");

            RuleFor(x => x.AttachmentId)
                .ValidGuid("AttachmentId");

            RuleFor(x => x.InstructorId)
                .ValidGuid("InstructorId");
        }
    }
}
