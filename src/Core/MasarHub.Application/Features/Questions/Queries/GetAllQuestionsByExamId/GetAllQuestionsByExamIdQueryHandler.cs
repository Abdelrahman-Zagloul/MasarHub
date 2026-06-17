using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Questions.Queries.GetQuestionById;
using MediatR;

namespace MasarHub.Application.Features.Questions.Queries.GetAllQuestionsByExamId
{
    public sealed class GetAllQuestionsByExamIdQueryHandler : IRequestHandler<GetAllQuestionsByExamIdQuery, Result<IReadOnlyList<QuestionDetailsResponse>>>
    {
        private readonly IQuestionQuery _questionQuery;

        public GetAllQuestionsByExamIdQueryHandler(IQuestionQuery questionQuery)
        {
            _questionQuery = questionQuery;
        }

        public async Task<Result<IReadOnlyList<QuestionDetailsResponse>>> Handle(GetAllQuestionsByExamIdQuery request, CancellationToken cancellationToken)
        {
            var questions = await _questionQuery.GetAllQuestionsByExamIdAsync(request, cancellationToken);

            var response = questions.Select(q => new QuestionDetailsResponse
            (
                q.Id,
                q.ExamId,
                q.QuestionText,
                q.QuestionMark,
                q.QuestionType,
                q.Options.Select(o => new OptionResponse(o.Id, o.QuestionId, o.Text, o.IsCorrect)).ToList()
            )).ToList();

            return response;
        }
    }
}