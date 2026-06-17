using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Questions.Commands.UpdateQuestion
{
    public sealed class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
    {
        public UpdateQuestionCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .ValidGuid("ExamId");

            RuleFor(x => x.QuestionId)
                .ValidGuid("QuestionId");

            RuleFor(x => x.QuestionText)
                .ValidMinLength(10, "QuestionText")
                .ValidMaxLength(1000, "QuestionText");

            RuleFor(x => x.QuestionMark)
                .ValidGreaterThanZero("QuestionMark");

            When(x => x.Options != null, () =>
            {
                RuleFor(x => x.Options)
                    .RequiredNonEmptyCollection("Options");

                RuleForEach(x => x.Options)
                   .ChildRules(option =>
                   {
                       option.RuleFor(x => x.OptionId)
                           .ValidGuid("OptionId");

                       option.RuleFor(x => x.Text)
                           .ValidMinLength(2, "OptionText")
                           .ValidMaxLength(500, "OptionText");
                   });
            });
        }
    }
}
