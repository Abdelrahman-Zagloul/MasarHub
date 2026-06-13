using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Lessons.Commands.CompleteVideoLesson
{
    public sealed class CompleteVideoLessonCommandValidator : AbstractValidator<CompleteVideoLessonCommand>
    {
        public CompleteVideoLessonCommandValidator()
        {
            RuleFor(x => x.Title)
              .Required("Title")
              .MinLengthValidation(10, "Title")
              .MaxLengthValidation(200, "Title");

            RuleFor(x => x.Description)
                .MinLengthValidation(10, "Description")
                .MaxLengthValidation(1000, "Description");

            RuleFor(x => x.FileKey)
                .Required("FileKey")
                .MinLengthValidation(10, "FileKey")
                .MaxLengthValidation(200, "FileKey");
        }
    }
}
