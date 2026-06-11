using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses.Lessons;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.DeleteLesson
{
    public sealed class DeleteLessonCommandHandler : IRequestHandler<DeleteLessonCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILessonQuery _lessonQuery;
        private readonly IRepository<Lesson> _lessonRepository;

        public DeleteLessonCommandHandler(IUnitOfWork unitOfWork, ILessonQuery lessonQuery, IRepository<Lesson> lessonRepository)
        {
            _unitOfWork = unitOfWork;
            _lessonQuery = lessonQuery;
            _lessonRepository = lessonRepository;
        }

        public async Task<Result> Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
        {
            var courseState = await _lessonQuery.GetCourseStateAsync(
                 request.CourseId, request.ModuleId, request.InstructorId, cancellationToken);

            if (!courseState.IsOwner)
                return Error.Forbidden("course.access_denied");

            if (!courseState.ModuleExist)
                return Error.NotFound("module.not_found");

            var lesson = await _lessonRepository.GetAsync(x => x.Id == request.LessonId && x.ModuleId == request.ModuleId);
            if (lesson == null)
                return Error.NotFound("lesson.not_found");

            var deletedResult = lesson.Delete(courseState.CourseStatus);
            if (deletedResult.IsFailure)
                return Error.Conflict(deletedResult.Error.Code);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
