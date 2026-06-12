using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Courses.Lessons;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.UpdateLesson
{
    public sealed class UpdateLessonCommandHandler : IRequestHandler<UpdateLessonCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILessonQuery _lessonQuery;
        private readonly IRepository<Lesson> _lessonRepository;

        public UpdateLessonCommandHandler(IUnitOfWork unitOfWork, ILessonQuery lessonQuery, IRepository<Lesson> lessonRepository)
        {
            _unitOfWork = unitOfWork;
            _lessonQuery = lessonQuery;
            _lessonRepository = lessonRepository;
        }

        public async Task<Result> Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
        {
            var courseState = await _lessonQuery.GetCourseStateAsync(request.ModuleId, request.InstructorId, cancellationToken);

            if (!courseState.ModuleExist)
                return Error.NotFound("module.not_found");

            if (!courseState.IsOwner)
                return Error.Forbidden("course.access_denied");

            var lesson = await _lessonRepository.GetAsync(x => x.Id == request.LessonId && x.ModuleId == request.ModuleId);
            if (lesson == null)
                return Error.NotFound("lesson.not_found");

            DomainResult updateResult = DomainResult.Success();

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                updateResult = lesson.UpdateTitle(request.Title);
                if (updateResult.IsFailure)
                    return Error.BadRequest(updateResult.Error.Code);
            }

            if (request.Description != null)
                updateResult = lesson.UpdateDescription(request.Description);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
