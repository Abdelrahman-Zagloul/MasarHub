using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Exams;
using MediatR;

namespace MasarHub.Application.Features.Questions.Commands.CreateQuestion
{
    public sealed class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Result<CreateQuestionResponse>>
    {
        private readonly IExamQuery _examQuery;
        private readonly IRepository<Exam> _examRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Question> _questionRepository;
        public CreateQuestionCommandHandler(IExamQuery examQuery, IRepository<Exam> examRepository, IUnitOfWork unitOfWork, IRepository<Question> questionRepository)
        {
            _examQuery = examQuery;
            _examRepository = examRepository;
            _unitOfWork = unitOfWork;
            _questionRepository = questionRepository;
        }

        public async Task<Result<CreateQuestionResponse>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            var examData = await _examQuery.GetUpdateDataAsync(request.ExamId, request.InstructorId, cancellationToken);

            if (!examData.ExamExists)
                return Error.NotFound("exam.not_found");

            if (!examData.IsOwner)
                return Error.Forbidden("course.access_denied");

            var exam = await _examRepository.GetByIdAsync(request.ExamId, cancellationToken);
            if (exam is null)
                return Error.NotFound("exam.not_found");

            var questionResult = Question.Create(
                request.ExamId,
                request.QuestionText,
                request.QuestionMark,
                request.QuestionType,
                request.Options);

            if (questionResult.IsFailure)
                return questionResult.Error;

            var question = questionResult.Value;

            var addResult = exam.AddQuestion(question);
            if (addResult.IsFailure)
                return addResult.Error;

            await _questionRepository.AddAsync(question);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new CreateQuestionResponse(
                question.Id,
                question.ExamId,
                question.QuestionText,
                question.QuestionMark,
                question.QuestionType,
                question.Options.Select(o => new OptionResponse(o.Id, o.Text, o.IsCorrect)).ToList());
        }
    }
}
