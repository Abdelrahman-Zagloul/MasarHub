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
               .MinLengthValidation(10, "Title")
               .MaxLengthValidation(200, "Title");

            RuleFor(x => x.Description)
                .MinLengthValidation(10, "Description")
                .MaxLengthValidation(1000, "Description");
        }
    }
}
