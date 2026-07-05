using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed class ModuleProgress : SoftDeletableEntity
    {
        public Guid UserId { get; private set; }
        public Guid CourseId { get; private set; }
        public Guid ModuleId { get; private set; }
        public int CompletedLessons { get; private set; }
        public int TotalLessons { get; private set; }
        public DateTimeOffset? CompletedAt { get; private set; }
        private ModuleProgress() { }

        private ModuleProgress(Guid userId, Guid courseId, Guid moduleId, int totalLessons)
        {
            UserId = userId;
            CourseId = courseId;
            ModuleId = moduleId;
            TotalLessons = totalLessons;
        }
        public static DomainResult<ModuleProgress> Create(Guid userId, Guid courseId, Guid moduleId, int totalLessons)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(userId, nameof(userId)),
                Guard.AgainstEmptyGuid(courseId, nameof(courseId)),
                Guard.AgainstEmptyGuid(moduleId, nameof(moduleId)),
                Guard.AgainstNegativeOrZero(totalLessons, nameof(totalLessons))
            );

            if (error is not null)
                return error;

            return new ModuleProgress(userId, courseId, moduleId, totalLessons);
        }
        public DomainResult UpdateTotalLessons(int totalLessons)
        {
            var error = Guard.AgainstNegativeOrZero(totalLessons, nameof(totalLessons));
            if (error != DomainError.None)
                return error;

            TotalLessons = totalLessons;
            if (CompletedLessons > TotalLessons)
                CompletedLessons = TotalLessons;

            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult MarkLessonCompleted()
        {
            CompletedLessons++;

            if (CompletedLessons == TotalLessons)
                CompletedAt = DateTimeOffset.UtcNow;

            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult Reset()
        {
            CompletedLessons = 0;
            CompletedAt = null;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult Delete() => MarkAsDeleted();
    }
}
