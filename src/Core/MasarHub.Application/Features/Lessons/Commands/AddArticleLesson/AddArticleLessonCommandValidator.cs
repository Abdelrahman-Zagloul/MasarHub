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
                .ValidMinLength(10, "Title")
                .ValidMaxLength(200, "Title");

            RuleFor(x => x.Description)
                .ValidMinLength(10, "Description")
                .ValidMaxLength(1000, "Description");

            RuleFor(x => x.Content)
                .Required("Content")
                .ValidMinLength(10, "Content");
        }
    }
}
