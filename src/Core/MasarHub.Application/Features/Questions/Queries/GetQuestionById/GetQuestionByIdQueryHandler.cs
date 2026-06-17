using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Questions.Queries.GetQuestionById
{
    public sealed class GetQuestionByIdQueryHandler : IRequestHandler<GetQuestionByIdQuery, Result<QuestionDetailsResponse>>
    {
        private readonly IQuestionQuery _questionQuery;

        public GetQuestionByIdQueryHandler(IQuestionQuery questionQuery)
        {
            _questionQuery = questionQuery;
        }

        public async Task<Result<QuestionDetailsResponse>> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
        {
            var question = await _questionQuery.GetQuestionByIdAsync(request.QuestionId, request.ExamId, request.InstructorId, cancellationToken);
            if (question is null)
                return Error.NotFound("question.not_found");

            return new QuestionDetailsResponse
            (
                question.Id,
                question.ExamId,
                question.QuestionText,
                question.QuestionMark,
                question.QuestionType,
                question.Options.Select(o => new OptionResponse(o.Id, request.QuestionId, o.Text, o.IsCorrect)).ToList()
            );
        }
    }
}
