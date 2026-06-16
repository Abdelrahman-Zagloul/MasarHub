using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Lessons.Commands.AddArticleLesson
{
    public sealed class AddArticleLessonCommandValidator : AbstractValidator<AddArticleLessonCommand>
    {
        public AddArticleLessonCommandValidator()
        {
            RuleFor(x => x.Title)
                .Required("Title")
                .MinLengthValidation(10, "Title")
                .MaxLengthValidation(200, "Title");

            RuleFor(x => x.Description)
                .MinLengthValidation(10, "Description")
                .MaxLengthValidation(1000, "Description");

            RuleFor(x => x.Content)
                .Required("Content")
                .MinLengthValidation(10, "Content");
        }
    }
}
