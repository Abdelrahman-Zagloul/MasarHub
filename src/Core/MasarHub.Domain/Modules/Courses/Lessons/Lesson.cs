using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Courses.Events;

namespace MasarHub.Domain.Modules.Courses.Lessons
{
    public abstract class Lesson : SoftDeletableEntity
    {
        public string Title { get; protected set; } = null!;
        public string? Description { get; protected set; }
        public int DisplayOrder { get; protected set; }
        public bool IsPreviewable { get; protected set; }
        public Guid ModuleId { get; protected set; }
        public LessonStatus LessonStatus { get; private set; }
        protected Lesson() { }

        protected Lesson(Guid moduleId, bool isPreviewable, string title, int order, string? description)
        {
            ModuleId = moduleId;
            IsPreviewable = isPreviewable;
            Title = title;
            DisplayOrder = order;
            Description = description;
            LessonStatus = LessonStatus.Active;
        }

        public DomainResult UpdateTitle(string title)
        {
            var error = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            if (error != DomainError.None)
                return error;

            Title = title;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateDescription(string? description)
        {
            Description = description;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult EnablePreview()
        {
            if (IsPreviewable)
                return new DomainError("lesson.preview_already_enabled");

            IsPreviewable = true;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult DisablePreview()
        {
            if (!IsPreviewable)
                return new DomainError("lesson.preview_already_disabled");

            IsPreviewable = false;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult Archive(CourseStatus courseStatus)
        {
            if (courseStatus != CourseStatus.Published)
                return new DomainError("lesson.cannot_archive_unpublished_lesson");

            if (LessonStatus == LessonStatus.Archived)
                return new DomainError("lesson.already_archived");

            LessonStatus = LessonStatus.Archived;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult Unarchive(CourseStatus courseStatus)
        {
            if (LessonStatus != LessonStatus.Archived)
                return new DomainError("lesson.already_not_archived");

            LessonStatus = LessonStatus.Active;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult ChangeDisplayOrder(int displayOrder)
        {
            var error = Guard.AgainstNegativeOrZero(displayOrder, nameof(displayOrder));
            if (error != DomainError.None)
                return error;

            DisplayOrder = displayOrder;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult Delete(CourseStatus courseStatus)
        {
            if (courseStatus == CourseStatus.Published)
                return new DomainError("lesson.cannot_delete_published_lesson");

            DisplayOrder = 0;
            MarkAsDeleted();
            RaiseDomainEvent(new LessonDeletedDomainEvent(ModuleId, Id));
            return DomainResult.Success();
        }


        protected static DomainError ValidateLesson(Guid moduleId, string title, int order)
        {
            return GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(moduleId, nameof(moduleId)),
                Guard.AgainstNullOrWhiteSpace(title, nameof(title)),
                Guard.AgainstNegativeOrZero(order, nameof(order))
            ) ?? DomainError.None;
        }
    }
}
