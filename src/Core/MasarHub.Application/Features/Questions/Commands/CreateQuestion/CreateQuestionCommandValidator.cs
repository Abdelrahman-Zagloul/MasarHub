using FluentValidation;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Exams;

namespace MasarHub.Application.Features.Questions.Commands.CreateQuestion
{
    public sealed class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
    {
        public CreateQuestionCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .ValidGuid("ExamId");

            RuleFor(x => x.QuestionText)
                .Required("QuestionText")
                .ValidMinLength(10, "QuestionText")
                .ValidMaxLength(1000, "QuestionText");

            RuleFor(x => x.QuestionMark)
                .ValidGreaterThanZero("QuestionMark");

            RuleFor(x => x.QuestionType)
                .ValidEnum("QuestionType");

            RuleFor(x => x.Options)
                .RequiredNonEmptyCollection("Options");

            RuleFor(x => x.Options)
                .Must(HaveUniqueOptionTexts)
                .WithErrorCode("validation.duplicate_option_text")
                .WithName("Options");

            RuleForEach(x => x.Options)
                .ChildRules(rule =>
                {
                    rule.RuleFor(x => x.Text)
                        .Required("OptionText")
                        .ValidMaxLength(500, "OptionText");
                });


        }
        private static bool HaveUniqueOptionTexts(IEnumerable<Question.OptionInput>? options)
        {
            if (options is null)
                return true;

            return options
                .Select(o => o.Text.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() == options.Count();
        }
    }
}
