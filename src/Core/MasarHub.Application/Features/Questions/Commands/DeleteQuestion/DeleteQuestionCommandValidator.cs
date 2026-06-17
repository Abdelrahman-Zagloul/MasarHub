using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Questions.Commands.DeleteQuestion
{
    public sealed class DeleteQuestionCommandValidator : AbstractValidator<DeleteQuestionCommand>
    {
        public DeleteQuestionCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .ValidGuid("ExamId");

            RuleFor(x => x.QuestionId)
                .ValidGuid("QuestionId");
        }
    }
}
