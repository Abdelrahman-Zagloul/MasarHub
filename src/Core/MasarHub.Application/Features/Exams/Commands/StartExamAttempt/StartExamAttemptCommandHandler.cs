using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Exams;
using MediatR;

namespace MasarHub.Application.Features.Exams.Commands.StartExamAttempt
{
    public sealed class StartExamAttemptCommandHandler : IRequestHandler<StartExamAttemptCommand, Result<StartExamAttemptResponse>>
    {
        private readonly IExamQuery _examQuery;
        private readonly IRepository<ExamAttempt> _examAttemptRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StartExamAttemptCommandHandler(IExamQuery examQuery, IRepository<ExamAttempt> examAttemptRepository, IUnitOfWork unitOfWork)
        {
            _examQuery = examQuery;
            _examAttemptRepository = examAttemptRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<StartExamAttemptResponse>> Handle(StartExamAttemptCommand request, CancellationToken cancellationToken)
        {
            var data = await _examQuery.GetAttemptStartDataAsync(request.ExamId, request.CourseId, request.UserId, cancellationToken);
            if (data == null || !data.IsEnrolled)
                return Error.Forbidden("course.not_enrolled");

            var exam = await _examQuery.GetExamDetailsAsync(request.ExamId, cancellationToken);
            if (exam == null || !exam.IsPublished)
                return Error.NotFound("exam.not_found");

            if (data.CompletedAttempts >= exam.MaxAttempts)
                return Error.BadRequest("exam_attempt.max_attempts_reached");

            if (data.InProgressAttemptId.HasValue)
            {
                var inProgressAttempt = await _examAttemptRepository.GetByIdAsync(data.InProgressAttemptId.Value, cancellationToken);
                if (inProgressAttempt != null)
                    return MapToResponse(inProgressAttempt, exam);
            }

            var attempt = ExamAttempt.Create(request.ExamId, request.UserId).Value;
            await _examAttemptRepository.AddAsync(attempt, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToResponse(attempt, exam);
        }

        private static StartExamAttemptResponse MapToResponse(ExamAttempt attempt, Exam exam)
        {
            return new StartExamAttemptResponse
            (
                attempt.Id,
                attempt.StartedAt,
                exam.Title,
                exam.DurationInMinutes,
                exam.Questions.Sum(q => q.QuestionMark),
                exam.Questions.Select(q => new QuestionResponse
                (
                    q.Id,
                    q.QuestionText,
                    q.QuestionMark,
                    q.QuestionType,
                    q.Options.Select(o => new ExamOptionResponse(o.Id, o.Text)).ToList()
                )).ToList()
            );
        }
    }
}
