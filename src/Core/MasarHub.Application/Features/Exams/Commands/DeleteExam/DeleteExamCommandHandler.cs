using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Exams;
using MediatR;

namespace MasarHub.Application.Features.Exams.Commands.DeleteExam
{
    public sealed class DeleteExamCommandHandler : IRequestHandler<DeleteExamCommand, Result>
    {
        private readonly IExamQuery _examQuery;
        private readonly IRepository<Exam> _examRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteExamCommandHandler(IExamQuery examQuery, IRepository<Exam> examRepository, IUnitOfWork unitOfWork)
        {
            _examQuery = examQuery;
            _examRepository = examRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteExamCommand request, CancellationToken cancellationToken)
        {
            var examState = await _examQuery.GetExamStateAsync(request.ExamId, request.InstructorId, cancellationToken);

            if (!examState.ExamExists)
                return Error.NotFound("exam.not_found");

            if (!examState.IsOwner)
                return Error.Forbidden("course.access_denied");

            var exam = await _examRepository.GetByIdAsync(request.ExamId, cancellationToken);
            if (exam == null)
                return Error.NotFound("exam.not_found");

            var deleteResult = exam.Delete(examState.HasAttempts);
            if (deleteResult.IsFailure)
                return Error.Conflict(deleteResult.Error.Code);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
