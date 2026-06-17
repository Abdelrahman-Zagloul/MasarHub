using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Questions.Queries.GetAllQuestionsByExamId
{
    public sealed class GetAllQuestionsByExamIdQueryValidator : AbstractValidator<GetAllQuestionsByExamIdQuery>
    {
        public GetAllQuestionsByExamIdQueryValidator()
        {
            RuleFor(x => x.ExamId)
                .ValidGuid("ExamId");

            RuleFor(x => x.InstructorId)
                .ValidGuid("InstructorId");

            RuleFor(x => x.QuestionType)
                .ValidEnum("QuestionType");
        }
    }
}