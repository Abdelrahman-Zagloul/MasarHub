using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Common.ValueObjects;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed record CourseRequirement : ValueObject
    {
        public string Value { get; } = null!;

        private CourseRequirement() { }

        private CourseRequirement(string value)
        {
            Value = value;
        }

        public static Result<CourseRequirement> Create(string value)
        {
            var error = Guard.AgainstNullOrWhiteSpace(value, nameof(value));
            if (error is not null)
                return error;

            return new CourseRequirement(value);
        }
    }
}
