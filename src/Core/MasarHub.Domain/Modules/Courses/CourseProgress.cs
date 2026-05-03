using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

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
            UserId = userId;
            CourseId = courseId;
            TotalLessons = totalLessons;
        }

        public static Result<CourseProgress> Create(Guid userId, Guid courseId, int totalLessons)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(userId, nameof(userId)),
                Guard.AgainstEmptyGuid(courseId, nameof(courseId)),
                Guard.AgainstNegativeOrZero(totalLessons, nameof(totalLessons))
            );

            if (error is not null)
                return error;

            return new CourseProgress(userId, courseId, totalLessons);
        }

        public Result UpdateTotalLessons(int totalLessons)
        {
            var error = Guard.AgainstNegativeOrZero(totalLessons, nameof(totalLessons));
            if (error is not null)
                return error;

            TotalLessons = totalLessons;
            if (CompletedLessons > TotalLessons)
                CompletedLessons = TotalLessons;

            Recalculate();
            MarkAsUpdated();
            return Result.Success();
        }

        public Result UpdateProgress(int completedLessons)
        {
            var error = Guard.AgainstNegative(completedLessons, nameof(completedLessons));
            if (error is not null)
                return error;

            CompletedLessons = completedLessons;
            if (CompletedLessons > TotalLessons)
                CompletedLessons = TotalLessons;

            Recalculate();
            MarkAsUpdated();
            return Result.Success();
        }

        public Result Reset()
        {
            CompletedLessons = 0;
            ProgressPercentage = 0;
            IsCompleted = false;
            CompletedAt = null;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result Delete() => MarkAsDeleted();

        private void Recalculate()
        {
            ProgressPercentage = Math.Round((decimal)CompletedLessons / TotalLessons * 100, 2);

            if (CompletedLessons >= TotalLessons)
            {
                CompletedLessons = TotalLessons;
                IsCompleted = true;
                CompletedAt ??= DateTimeOffset.UtcNow;
            }
            else
            {
                IsCompleted = false;
                CompletedAt = null;
            }
        }
    }
}
