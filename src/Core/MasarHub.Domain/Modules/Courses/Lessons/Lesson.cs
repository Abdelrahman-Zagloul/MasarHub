using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;

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
            ModuleId = Guard.AgainstEmptyGuid(moduleId, nameof(moduleId));
            Title = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            DisplayOrder = Guard.AgainstNegativeOrZero(order, nameof(order));
            Description = description;
        }

        public void UpdateTitle(string title)
        {
            Title = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            MarkAsUpdated();
        }

        public void UpdateDescription(string? description)
        {
            Description = description;
            MarkAsUpdated();
        }
        public void ChangeOrder(int order)
        {
            DisplayOrder = Guard.AgainstNegativeOrZero(order, nameof(order));
            MarkAsUpdated();
        }
        public void EnablePreview()
        {
            IsPreviewable = true;
            MarkAsUpdated();
        }
        public void DisablePreview()
        {
            IsPreviewable = false;
            MarkAsUpdated();
        }

        public void Delete()
        {
            MarkAsDeleted();
        }
    }
}
