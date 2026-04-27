using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.ValueObjects;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed record CourseRequirement : ValueObject
    {
        public string Value { get; } = null!;
        private CourseRequirement() { }
        private CourseRequirement(string value)
        {
            Value = Guard.AgainstNullOrWhiteSpace(value, nameof(value));
        }

        public static CourseRequirement Create(string value)
        {
            return new CourseRequirement(value);
        }
    }
}
