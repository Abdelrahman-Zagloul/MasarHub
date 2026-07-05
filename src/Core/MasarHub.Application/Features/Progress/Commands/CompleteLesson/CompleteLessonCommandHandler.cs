using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses.Lessons;
using MediatR;

namespace MasarHub.Application.Features.Progress.Commands.CompleteLesson
{
    public sealed class CompleteLessonCommandHandler : IRequestHandler<CompleteLessonCommand, Result>
    {
        private readonly IProgressQuery _progressQuery;
        private readonly IRepository<LessonProgress> _lessonProgressRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompleteLessonCommandHandler(IProgressQuery progressQuery, IRepository<LessonProgress> lessonProgressRepository, IUnitOfWork unitOfWork)
        {
            _progressQuery = progressQuery;
            _lessonProgressRepository = lessonProgressRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CompleteLessonCommand request, CancellationToken cancellationToken)
        {
            var data = await _progressQuery.GetCreateProgressDataAsync(request.UserId, request.CourseId, request.LessonId, cancellationToken);

            if (!data.HasEnrollment)
                return Error.Forbidden("course.not_enrolled");

            if (!data.LessonExists)
                return Error.NotFound("lesson.not_found");

            if (data.AlreadyCompleted)
                return Error.Conflict("progress.already_completed");

            var createResult = LessonProgress.Create(request.UserId, request.LessonId, data.ModuleId!.Value, request.CourseId);
            if (createResult.IsFailure)
                return createResult.Error;

            await _lessonProgressRepository.AddAsync(createResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
