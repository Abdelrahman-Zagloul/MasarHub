using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Exams;
using MediatR;

namespace MasarHub.Application.Features.Questions.Commands.UpdateQuestion
{
    public sealed class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, Result>
    {
        private readonly IExamQuery _examQuery;
        private readonly IRepository<Question> _questionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateQuestionCommandHandler(IExamQuery examQuery, IRepository<Question> questionRepository, IUnitOfWork unitOfWork)
        {
            _examQuery = examQuery;
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            var examData = await _examQuery.GetUpdateDataAsync(request.ExamId, request.InstructorId, cancellationToken);

            if (!examData.ExamExists)
                return Error.NotFound("exam.not_found");

            if (!examData.IsOwner)
                return Error.Forbidden("course.access_denied");

            var question = await _questionRepository
                .GetAsync(x => x.Id == request.QuestionId && x.ExamId == request.ExamId, cancellationToken, x => x.Options);
            if (question == null)
                return Error.NotFound("question.not_found");

            if (request.QuestionText != null)
            {
                var textResult = question.UpdateQuestionText(request.QuestionText);
                if (textResult.IsFailure)
                    return textResult.Error;
            }

            if (request.QuestionMark.HasValue)
            {
                var markResult = question.UpdateQuestionMark(request.QuestionMark.Value);
                if (markResult.IsFailure)
                    return markResult.Error;
            }

            if (request.Options != null)
            {
                var optionsResult = question.UpdateOptions(request.Options);
                if (optionsResult.IsFailure)
                {
                    var domainError = optionsResult.Error;
                    if (domainError.Code == "exam.option_not_found")
                    {
                        var optionId = domainError.PropertyName?.Replace("Option:", "");
                        return Error.BadRequest(domainError.Code,
                            metadata: new() { ["OptionId"] = optionId! });
                    }
                    return domainError;
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
