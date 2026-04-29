using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;

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

        private LessonProgress(
            Guid userId,
            Guid lessonId,
            Guid moduleId,
            Guid courseId)
        {
            UserId = Guard.AgainstEmptyGuid(userId, nameof(userId));
            LessonId = Guard.AgainstEmptyGuid(lessonId, nameof(lessonId));
            ModuleId = Guard.AgainstEmptyGuid(moduleId, nameof(moduleId));
            CourseId = Guard.AgainstEmptyGuid(courseId, nameof(courseId));
        }

        public static LessonProgress Create(
            Guid userId,
            Guid lessonId,
            Guid moduleId,
            Guid courseId)
        {
            return new LessonProgress(userId, lessonId, moduleId, courseId);
        }

        public void MarkCompleted()
        {
            if (IsCompleted) return;

            IsCompleted = true;
            CompletedAt = DateTimeOffset.UtcNow;
        }

        public void Reset()
        {
            IsCompleted = false;
            CompletedAt = null;
        }
    }
}