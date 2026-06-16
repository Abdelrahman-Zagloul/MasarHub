using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Exams;
using MediatR;

namespace MasarHub.Application.Features.Exams.Commands.UpdateExam
{
    public sealed class UpdateExamCommandHandler : IRequestHandler<UpdateExamCommand, Result>
    {
        private readonly IExamQuery _examQuery;
        private readonly IRepository<Exam> _examRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateExamCommandHandler(IExamQuery examQuery, IRepository<Exam> examRepository, IUnitOfWork unitOfWork)
        {
            _examQuery = examQuery;
            _examRepository = examRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateExamCommand request, CancellationToken cancellationToken)
        {
            var updateData = await _examQuery.GetUpdateDataAsync(request.ExamId, request.InstructorId, cancellationToken);

            if (!updateData.ExamExists)
                return Error.NotFound("exam.not_found");

            if (!updateData.IsOwner)
                return Error.Forbidden("course.access_denied");

            var exam = await _examRepository.GetByIdAsync(request.ExamId, cancellationToken);
            if (exam is null)
                return Error.NotFound("exam.not_found");

            if (request.Title != null)
            {
                var titleResult = exam.UpdateTitle(request.Title);
                if (titleResult.IsFailure)
                    return titleResult.Error;
            }

            if (request.Description != null)
                exam.UpdateDescription(request.Description);

            if (request.MaxAttempts.HasValue)
            {
                var maxAttemptResult = exam.UpdateMaxAttempts(request.MaxAttempts.Value);
                if (maxAttemptResult.IsFailure)
                    return maxAttemptResult.Error;
            }

            if (request.PassingScorePercentage.HasValue)
            {
                var scoreResult = exam.UpdatePassingScore(request.PassingScorePercentage.Value);
                if (scoreResult.IsFailure)
                    return scoreResult.Error;
            }

            var durationResult = exam.UpdateDuration(request.DurationMinutes);
            if (durationResult.IsFailure)
                return durationResult.Error;


            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
