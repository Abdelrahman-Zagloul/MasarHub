using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed class CourseModule : SoftDeletableEntity
    {
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }
        public int DisplayOrder { get; private set; }

        public Guid CourseId { get; private set; }

        private CourseModule() { }

        private CourseModule(Guid courseId, string title, int displayOrder, string? description)
        {
            CourseId = Guard.AgainstEmptyGuid(courseId, nameof(courseId));
            Title = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            DisplayOrder = Guard.AgainstNegativeOrZero(displayOrder, nameof(displayOrder));
            Description = description;
        }

        public static CourseModule Create(
            Guid courseId,
            string title,
            int displayOrder,
            string? description = null)
        {
            return new CourseModule(courseId, title, displayOrder, description);
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

        public void Delete()
        {
            MarkAsDeleted();
        }
    }
}