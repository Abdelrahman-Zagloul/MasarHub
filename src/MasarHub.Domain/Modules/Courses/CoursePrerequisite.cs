using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.ValueObjects;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed record CoursePrerequisite : ValueObject
    {
        public string Value { get; } = null!;
        private CoursePrerequisite() { }
        private CoursePrerequisite(string value)
        {
            Value = Guard.AgainstNullOrWhiteSpace(value, nameof(value));
        }

        public static CoursePrerequisite Create(string value)
        {
            return new CoursePrerequisite(value);
        }
    }
}
