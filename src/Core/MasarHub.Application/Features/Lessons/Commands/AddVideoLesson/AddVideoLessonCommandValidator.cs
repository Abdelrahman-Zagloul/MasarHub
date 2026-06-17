using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Lessons.Commands.AddVideoLesson
{
    public sealed class AddVideoLessonCommandValidator : AbstractValidator<AddVideoLessonCommand>
    {
        public AddVideoLessonCommandValidator()
        {
            RuleFor(x => x.Title)
              .Required("Title")
              .ValidMinLength(10, "Title")
              .ValidMaxLength(200, "Title");

            RuleFor(x => x.Description)
                .ValidMinLength(10, "Description")
                .ValidMaxLength(1000, "Description");

            RuleFor(x => x.FileKey)
                .Required("FileKey")
                .ValidMinLength(10, "FileKey")
                .ValidMaxLength(200, "FileKey");
        }
    }
}
