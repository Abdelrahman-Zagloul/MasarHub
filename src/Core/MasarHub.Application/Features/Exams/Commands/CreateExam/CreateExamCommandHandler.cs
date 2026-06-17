using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Exams;
using MediatR;

namespace MasarHub.Application.Features.Exams.Commands.CreateExam
{
    public sealed class CreateExamCommandHandler : IRequestHandler<CreateExamCommand, Result<CreateExamResponse>>
    {
        private readonly IExamQuery _examQuery;
        private readonly IRepository<Exam> _examRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateExamCommandHandler(IExamQuery examQuery, IRepository<Exam> examRepository, IUnitOfWork unitOfWork)
        {
            _examQuery = examQuery;
            _examRepository = examRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateExamResponse>> Handle(CreateExamCommand request, CancellationToken cancellationToken)
        {
            var creationData = await _examQuery.GetCreationDataAsync(request.CourseId, request.ModuleId, request.InstructorId, cancellationToken);

            if (!creationData.CourseExists)
                return Error.NotFound("course.not_found");

            if (!creationData.IsOwner)
                return Error.Forbidden("course.access_denied");

            if (request.ModuleId.HasValue && !creationData.ModuleExists)
                return Error.NotFound("module.not_found");

            var examResult = Exam.Create(
                request.CourseId,
                request.Title,
                request.PassingScorePercentage,
                request.MaxAttempts,
                request.ModuleId,
                request.Description,
                request.DurationMinutes);

            if (examResult.IsFailure)
                return examResult.Error;

            var exam = examResult.Value;
            await _examRepository.AddAsync(exam, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateExamResponse(
                exam.Id,
                exam.Title,
                exam.Description,
                exam.PassingScorePercentage,
                exam.MaxAttempts,
                exam.DurationInMinutes,
                exam.IsPublished,
                exam.CourseId,
                exam.ModuleId);
        }
    }
}
