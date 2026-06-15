using FluentAssertions;
using MasarHub.Application.Features.Courses.Commands.UpdateCourseRequirements;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.UpdateCourseRequirements
{
    [Trait("UnitTests.Feature.Courses", "UpdateCourseRequirements")]
    public sealed class UpdateCourseRequirementsCommandValidatorTests
    {
        private readonly UpdateCourseRequirementsCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new UpdateCourseRequirementsCommand(Guid.NewGuid(), ["Basic math", "PC access"]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var command = new UpdateCourseRequirementsCommand(Guid.Empty, ["Basic math"]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_NullCollection_ReturnsValidationError()
        {
            var command = new UpdateCourseRequirementsCommand(Guid.NewGuid(), null!);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Requirements");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_EmptyRequirement_ReturnsValidationError(string requirement)
        {
            var command = new UpdateCourseRequirementsCommand(Guid.NewGuid(), [requirement]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.required");
        }

        [Fact]
        public void Validate_RequirementTooLong_ReturnsValidationError()
        {
            var longRequirement = new string('A', 501);
            var command = new UpdateCourseRequirementsCommand(Guid.NewGuid(), [longRequirement]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }
    }
}
