using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Common.ValueObjects;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed record CoursePrerequisite : ValueObject
    {
        public string Value { get; } = null!;

        private CoursePrerequisite() { }

        private CoursePrerequisite(string value)
        {
            Value = value;
        }

        public static Result<CoursePrerequisite> Create(string value)
        {
            var error = Guard.AgainstNullOrWhiteSpace(value, nameof(value));
            if (error is not null)
                return error;

            return new CoursePrerequisite(value);
        }
    }
}
