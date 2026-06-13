using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Lessons.Commands.UpdateVideoThumbnail
{
    public sealed class UpdateVideoThumbnailCommandValidator : AbstractValidator<UpdateVideoThumbnailCommand>
    {
        public UpdateVideoThumbnailCommandValidator()
        {
            RuleFor(x => x.ModuleId)
                .ValidGuid("ModuleId");

            RuleFor(x => x.LessonId)
                .ValidGuid("LessonId");
        }
    }
}
