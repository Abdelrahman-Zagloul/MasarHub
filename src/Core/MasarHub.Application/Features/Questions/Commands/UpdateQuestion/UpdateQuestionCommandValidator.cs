using FluentValidation;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Exams;

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

            When(x => x.Options is not null, () =>
            {
                RuleFor(x => x.Options)
                    .RequiredNonEmptyCollection("Options");

                RuleForEach(x => x.Options)
                    .SetValidator(new OptionUpdateInputValidator());
            });
        }
    }

    public sealed class OptionUpdateInputValidator : AbstractValidator<Question.OptionUpdateInput>
    {
        public OptionUpdateInputValidator()
        {
            RuleFor(x => x.OptionId)
                .ValidGuid("OptionId");

            RuleFor(x => x.Text)
                .ValidMinLength(2, "OptionText")
                .ValidMaxLength(500, "OptionText");
        }
    }
}
