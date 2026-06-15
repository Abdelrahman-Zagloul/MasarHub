using FluentAssertions;
using MasarHub.Application.Features.Courses.Commands.UpdateCourseLearningObjective;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.UpdateCourseLearningObjective
{
    [Trait("UnitTests.Feature.Courses", "UpdateCourseLearningObjective")]
    public sealed class UpdateCourseLearningObjectiveCommandValidatorTests
    {
        private readonly UpdateCourseLearningObjectiveCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new UpdateCourseLearningObjectiveCommand(Guid.NewGuid(), ["Learn C#", "Master LINQ"]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var command = new UpdateCourseLearningObjectiveCommand(Guid.Empty, ["Learn C#"]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_NullCollection_ReturnsValidationError()
        {
            var command = new UpdateCourseLearningObjectiveCommand(Guid.NewGuid(), null!);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "LearningObjectives");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_EmptyObjective_ReturnsValidationError(string objective)
        {
            var command = new UpdateCourseLearningObjectiveCommand(Guid.NewGuid(), [objective]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.required");
        }

        [Fact]
        public void Validate_ObjectiveTooLong_ReturnsValidationError()
        {
            var longObjective = new string('A', 501);
            var command = new UpdateCourseLearningObjectiveCommand(Guid.NewGuid(), [longObjective]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }
    }
}
