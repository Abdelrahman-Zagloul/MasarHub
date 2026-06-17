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
                .ValidMinLength(1, "QuestionText")
                .ValidMaxLength(1000, "QuestionText");

            RuleFor(x => x.QuestionMark)
                .ValidGreaterThanZero("QuestionMark");

            RuleFor(x => x.QuestionType)
                .ValidEnum("QuestionType");

            RuleFor(x => x.Options)
                .RequiredNonEmptyCollection("Options");

            RuleForEach(x => x.Options)
                .SetValidator(new OptionInputValidator());
        }
    }

    public sealed class OptionInputValidator : AbstractValidator<Question.OptionInput>
    {
        public OptionInputValidator()
        {
            RuleFor(x => x.Text)
                .Required("OptionText")
                .ValidMaxLength(500, "OptionText");
        }
    }
}
