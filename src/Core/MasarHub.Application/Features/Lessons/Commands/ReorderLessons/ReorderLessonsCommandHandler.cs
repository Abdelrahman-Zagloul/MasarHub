using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses.Lessons;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.ReorderLessons
{
    public sealed class ReorderLessonsCommandHandler : IRequestHandler<ReorderLessonsCommand, Result>
    {
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly ILessonQuery _lessonQuery;
        private readonly IUnitOfWork _unitOfWork;

        public ReorderLessonsCommandHandler(IRepository<Lesson> lessonRepository, ILessonQuery lessonQuery, IUnitOfWork unitOfWork)
        {
            _lessonRepository = lessonRepository;
            _lessonQuery = lessonQuery;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ReorderLessonsCommand request, CancellationToken cancellationToken)
        {
            var reorderData = await _lessonQuery.GetReorderDataAsync(request.ModuleId, request.InstructorId, cancellationToken);

            if (!reorderData.ModuleExist)
                return Error.NotFound("module.not_found");

            if (!reorderData.IsOwner)
                return Error.Forbidden("course.access_denied");

            var lessons = await _lessonRepository.GetAllAsync(x => x.ModuleId == request.ModuleId, cancellationToken);

            if (lessons.Count != request.OrderedLessonIds.Count)
                return Error.BadRequest("lesson.reorder_items_mismatch");

            var lessonMap = lessons.ToDictionary(x => x.Id);
            for (int i = 0; i < request.OrderedLessonIds.Count; i++)
            {
                var lessonId = request.OrderedLessonIds[i];
                if (!lessonMap.TryGetValue(lessonId, out var lesson))
                    return Error.BadRequest("lesson.reorder_lesson_not_found");

                var result = lesson.ChangeDisplayOrder(i + 1);
                if (result.IsFailure)
                    return result.Error;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}