using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Courses.Lessons;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.ToggleLessonArchive
{
    public sealed class ToggleLessonArchiveCommandHandler : IRequestHandler<ToggleLessonArchiveCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILessonQuery _lessonQuery;
        private readonly IRepository<Lesson> _lessonRepository;

        public ToggleLessonArchiveCommandHandler(IUnitOfWork unitOfWork, ILessonQuery lessonQuery, IRepository<Lesson> lessonRepository)
        {
            _unitOfWork = unitOfWork;
            _lessonQuery = lessonQuery;
            _lessonRepository = lessonRepository;
        }

        public async Task<Result> Handle(ToggleLessonArchiveCommand request, CancellationToken cancellationToken)
        {
            var courseState = await _lessonQuery.GetCourseStateAsync(request.ModuleId, request.InstructorId, cancellationToken);

            if (!courseState.ModuleExist)
                return Error.NotFound("module.not_found");

            if (!courseState.IsOwner)
                return Error.Forbidden("course.access_denied");

            var lesson = await _lessonRepository.GetAsync(x => x.Id == request.LessonId && x.ModuleId == request.ModuleId);
            if (lesson == null)
                return Error.NotFound("lesson.not_found");

            DomainResult result = request.Archived
                ? lesson.Archive(courseState.CourseStatus)
                : lesson.Unarchive(courseState.CourseStatus);

            if (result.IsFailure)
                return Error.Conflict(result.Error.Code);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
