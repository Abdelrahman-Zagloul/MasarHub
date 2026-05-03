using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Courses.Lessons
{
    public sealed class LessonProgress : SoftDeletableEntity
    {
        public Guid UserId { get; private set; }
        public Guid LessonId { get; private set; }
        public Guid ModuleId { get; private set; }
        public Guid CourseId { get; private set; }
        public bool IsCompleted { get; private set; }
        public DateTimeOffset? CompletedAt { get; private set; }

        private LessonProgress() { }

        private LessonProgress(Guid userId, Guid lessonId, Guid moduleId, Guid courseId)
        {
            UserId = userId;
            LessonId = lessonId;
            ModuleId = moduleId;
            CourseId = courseId;
        }

        public static Result<LessonProgress> Create(Guid userId, Guid lessonId, Guid moduleId, Guid courseId)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(userId, nameof(userId)),
                Guard.AgainstEmptyGuid(lessonId, nameof(lessonId)),
                Guard.AgainstEmptyGuid(moduleId, nameof(moduleId)),
                Guard.AgainstEmptyGuid(courseId, nameof(courseId))
            );

            if (error is not null)
                return error;

            return new LessonProgress(userId, lessonId, moduleId, courseId);
        }

        public Result MarkCompleted()
        {
            if (IsCompleted)
                return Result.Success();

            IsCompleted = true;
            CompletedAt = DateTimeOffset.UtcNow;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result Reset()
        {
            IsCompleted = false;
            CompletedAt = null;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result Delete() => MarkAsDeleted();
    }
}
