using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.ValueObjects;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed record CourseLearningObjective : ValueObject
    {
        public string Value { get; } = null!;
        private CourseLearningObjective() { }
        private CourseLearningObjective(string value)
        {
            Value = Guard.AgainstNullOrWhiteSpace(value, nameof(value));
        }

        public static CourseLearningObjective Create(string value)
        {
            return new CourseLearningObjective(value);
        }
    }
}
