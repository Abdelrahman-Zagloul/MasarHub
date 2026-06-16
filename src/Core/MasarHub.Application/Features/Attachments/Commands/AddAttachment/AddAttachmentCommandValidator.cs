using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Attachments.Commands.AddAttachment
{
    public sealed class AddAttachmentCommandValidator : AbstractValidator<AddAttachmentCommand>
    {
        public AddAttachmentCommandValidator()
        {
            RuleFor(x => x.LessonId)
                .ValidGuid("LessonId");

            RuleFor(x => x.InstructorId)
                .ValidGuid("InstructorId");
        }
    }
}
