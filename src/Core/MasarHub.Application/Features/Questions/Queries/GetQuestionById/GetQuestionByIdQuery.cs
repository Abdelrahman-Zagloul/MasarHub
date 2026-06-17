using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Questions.Queries.GetQuestionById
{
    public sealed record GetQuestionByIdQuery(Guid ExamId, Guid QuestionId, Guid InstructorId) : IRequest<Result<QuestionDetailsResponse>>;
}
