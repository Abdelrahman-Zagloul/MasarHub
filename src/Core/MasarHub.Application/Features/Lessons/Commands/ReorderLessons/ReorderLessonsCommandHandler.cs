using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.ReorderLessons
{
    public sealed class ReorderLessonsCommandHandler : IRequestHandler<ReorderLessonsCommand, Result>
    {
        private readonly ILessonQuery _lessonQuery;
        public ReorderLessonsCommandHandler(ILessonQuery lessonQuery)
        {
            _lessonQuery = lessonQuery;
        }

        public async Task<Result> Handle(ReorderLessonsCommand request, CancellationToken cancellationToken)
        {
            var reorderData = await _lessonQuery.GetReorderDataAsync(request.ModuleId, request.InstructorId, cancellationToken);

            if (!reorderData.ModuleExist)
                return Error.NotFound("module.not_found");

            if (!reorderData.IsOwner)
                return Error.Forbidden("course.access_denied");

            var existingLessonIds = await _lessonQuery.GetLessonIdsByModuleIdAsync(request.ModuleId, cancellationToken);

            if (existingLessonIds.Count != request.OrderedLessonIds.Count)
                return Error.BadRequest("lesson.reorder_items_mismatch");

            var orderedLessonIdsSet = request.OrderedLessonIds.ToHashSet();
            if (!existingLessonIds.All(id => orderedLessonIdsSet.Contains(id)))
                return Error.BadRequest("lesson.reorder_lesson_not_found");

            bool isSuccess = await _lessonQuery.BulkUpdateDisplayOrderAsync(request.ModuleId, request.OrderedLessonIds, cancellationToken);
            if (!isSuccess)
                return Error.Failure("lesson.reorder_failed");

            return Result.Success();
        }
    }
}