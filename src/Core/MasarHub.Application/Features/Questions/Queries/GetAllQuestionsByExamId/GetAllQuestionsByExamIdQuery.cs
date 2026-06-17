using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Questions.Queries.GetQuestionById;
using MasarHub.Domain.Modules.Exams;
using MediatR;

namespace MasarHub.Application.Features.Questions.Queries.GetAllQuestionsByExamId
{
    public sealed record GetAllQuestionsByExamIdQuery
    (
        Guid ExamId,
        Guid InstructorId,
        QuestionType? QuestionType
    ) : IRequest<Result<IReadOnlyList<QuestionDetailsResponse>>>;
}