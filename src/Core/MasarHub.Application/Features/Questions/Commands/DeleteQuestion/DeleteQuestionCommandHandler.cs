using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Exams;
using MediatR;

namespace MasarHub.Application.Features.Questions.Commands.DeleteQuestion
{
    public sealed class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, Result>
    {
        private readonly IExamQuery _examQuery;
        private readonly IRepository<Exam> _examRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteQuestionCommandHandler(IExamQuery examQuery, IRepository<Exam> examRepository, IUnitOfWork unitOfWork)
        {
            _examQuery = examQuery;
            _examRepository = examRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            var examData = await _examQuery.GetUpdateDataAsync(request.ExamId, request.InstructorId, cancellationToken);

            if (!examData.ExamExists)
                return Error.NotFound("exam.not_found");

            if (!examData.IsOwner)
                return Error.Forbidden("course.access_denied");

            var exam = await _examRepository.GetAsync(x => x.Id == request.ExamId, cancellationToken, x => x.Questions);
            if (exam is null)
                return Error.NotFound("exam.not_found");

            var result = exam.RemoveQuestion(request.QuestionId);
            if (result.IsFailure)
                return result.Error;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
