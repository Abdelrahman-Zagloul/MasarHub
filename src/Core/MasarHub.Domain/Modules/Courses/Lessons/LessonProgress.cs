using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Courses.Events;

namespace MasarHub.Domain.Modules.Courses.Lessons
{
    public sealed class LessonProgress : SoftDeletableEntity
    {
        public Guid UserId { get; private set; }
        public Guid LessonId { get; private set; }
        public Guid ModuleId { get; private set; }
        public Guid CourseId { get; private set; }
        private LessonProgress() { }

        private LessonProgress(Guid userId, Guid lessonId, Guid moduleId, Guid courseId)
        {
            UserId = userId;
            LessonId = lessonId;
            ModuleId = moduleId;
            CourseId = courseId;
        }

        public static DomainResult<LessonProgress> Create(Guid userId, Guid lessonId, Guid moduleId, Guid courseId)
        {
            var error = GuardExtensions.FirstError
            (
                Guard.AgainstEmptyGuid(userId, nameof(userId)),
                Guard.AgainstEmptyGuid(lessonId, nameof(lessonId)),
                Guard.AgainstEmptyGuid(moduleId, nameof(moduleId)),
                Guard.AgainstEmptyGuid(courseId, nameof(courseId))
            );

            if (error != null)
                return error;

            var lessonProgress = new LessonProgress(userId, lessonId, moduleId, courseId);
            lessonProgress.RaiseDomainEvent(new LessonProgressCreatedDomainEvent(userId, courseId, moduleId, lessonId));
            return lessonProgress;
        }

        public DomainResult Delete() => MarkAsDeleted();
    }
}
