using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed class CourseProgress : SoftDeletableEntity
    {
        public Guid UserId { get; private set; }
        public Guid CourseId { get; private set; }
        public int CompletedLessons { get; private set; }
        public int TotalLessons { get; private set; }
        public decimal ProgressPercentage { get; private set; }
        public bool IsCompleted { get; private set; }
        public DateTimeOffset? CompletedAt { get; private set; }

        private CourseProgress() { }
        private CourseProgress(Guid userId, Guid courseId, int totalLessons)
        {
            UserId = Guard.AgainstEmptyGuid(userId, nameof(userId));
            CourseId = Guard.AgainstEmptyGuid(courseId, nameof(courseId));
            TotalLessons = Guard.AgainstNegativeOrZero(totalLessons, nameof(totalLessons));
        }

        public static CourseProgress Create(Guid userId, Guid courseId, int totalLessons)
        {
            return new CourseProgress(userId, courseId, totalLessons);
        }
        public void UpdateTotalLessons(int totalLessons)
        {
            TotalLessons = Guard.AgainstNegativeOrZero(totalLessons, nameof(totalLessons));

            if (CompletedLessons > TotalLessons)
                CompletedLessons = TotalLessons;

            Recalculate();
            MarkAsUpdated();
        }
        public void UpdateProgress(int completedLessons)
        {
            CompletedLessons = Guard.AgainstNegative(completedLessons, nameof(completedLessons));

            if (CompletedLessons > TotalLessons)
                CompletedLessons = TotalLessons;

            Recalculate();
            MarkAsUpdated();
        }
        public void Reset()
        {
            CompletedLessons = 0;
            ProgressPercentage = 0;
            IsCompleted = false;
            CompletedAt = null;

            MarkAsUpdated();
        }
        private void Recalculate()
        {
            if (TotalLessons == 0)
            {
                ProgressPercentage = 0;
                return;
            }

            ProgressPercentage = Math.Round((decimal)CompletedLessons / TotalLessons * 100, 2);

            if (CompletedLessons >= TotalLessons)
            {
                CompletedLessons = TotalLessons;
                IsCompleted = true;

                if (CompletedAt is null)
                    CompletedAt = DateTimeOffset.UtcNow;
            }
            else
            {
                IsCompleted = false;
                CompletedAt = null;
            }
        }
    }
}