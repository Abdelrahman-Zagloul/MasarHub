using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Courses.Events;

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
            CourseId = courseId;
            Title = title;
            DisplayOrder = displayOrder;
            Description = description;
        }

        public static DomainResult<CourseModule> Create(
            Guid courseId,
            string title,
            int displayOrder,
            string? description = null)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(courseId, nameof(courseId)),
                Guard.AgainstNullOrWhiteSpace(title, nameof(title)),
                Guard.AgainstNegativeOrZero(displayOrder, nameof(displayOrder))
            );
            if (error is not null)
                return error;

            var courseModule = new CourseModule(courseId, title, displayOrder, description);
            courseModule.RaiseDomainEvent(new ModuleCreatedDomainEvent(courseId, title));
            return courseModule;
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
        public DomainResult ChangeDisplayOrder(int displayOrder)
        {
            var error = Guard.AgainstNegativeOrZero(displayOrder, nameof(displayOrder));
            if (error != DomainError.None)
                return error;

            DisplayOrder = displayOrder;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult Delete()
        {
            DisplayOrder = 0;
            MarkAsDeleted();
            return DomainResult.Success();
        }
    }
}
