using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

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

        public static Result<CourseModule> Create(
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

            return new CourseModule(courseId, title, displayOrder, description);
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

        public Result Delete() => MarkAsDeleted();
    }
}
