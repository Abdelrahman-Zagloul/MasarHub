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

        protected Lesson(Guid moduleId, string title, int order, string? description)
        {
            ModuleId = moduleId;
            Title = title;
            DisplayOrder = order;
            Description = description;
        }

        public Result UpdateTitle(string title)
        {
            var error = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            if (error is not null)
                return error;

            Title = title;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result UpdateDescription(string? description)
        {
            Description = description;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result ChangeOrder(int order)
        {
            var error = Guard.AgainstNegativeOrZero(order, nameof(order));
            if (error is not null)
                return error;

            DisplayOrder = order;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result EnablePreview()
        {
            IsPreviewable = true;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result DisablePreview()
        {
            IsPreviewable = false;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result Delete() => MarkAsDeleted();

        protected static DomainError? ValidateLesson(Guid moduleId, string title, int order)
        {
            return GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(moduleId, nameof(moduleId)),
                Guard.AgainstNullOrWhiteSpace(title, nameof(title)),
                Guard.AgainstNegativeOrZero(order, nameof(order))
            );
        }
    }
}
