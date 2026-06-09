using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Courses.Lessons
{
    public abstract class Lesson : SoftDeletableEntity
    {
        public string Title { get; protected set; } = null!;
        public string? Description { get; protected set; }
        public int DisplayOrder { get; protected set; }
        public bool IsPreviewable { get; protected set; }
        public Guid ModuleId { get; protected set; }

        protected Lesson() { }

        protected Lesson(Guid moduleId, bool isPreviewable, string title, int order, string? description)
        {
            ModuleId = moduleId;
            IsPreviewable = isPreviewable;
            Title = title;
            DisplayOrder = order;
            Description = description;
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

        public DomainResult ChangeOrder(int order)
        {
            var error = Guard.AgainstNegativeOrZero(order, nameof(order));
            if (error != DomainError.None)
                return error;

            DisplayOrder = order;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult EnablePreview()
        {
            IsPreviewable = true;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult DisablePreview()
        {
            IsPreviewable = false;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult Delete() => MarkAsDeleted();

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
