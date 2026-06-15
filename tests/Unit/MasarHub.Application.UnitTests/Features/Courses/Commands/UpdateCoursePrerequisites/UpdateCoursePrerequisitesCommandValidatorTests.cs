using FluentAssertions;
using MasarHub.Application.Features.Courses.Commands.UpdateCoursePrerequisites;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.UpdateCoursePrerequisites
{
    [Trait("UnitTests.Feature.Courses", "UpdateCoursePrerequisites")]
    public sealed class UpdateCoursePrerequisitesCommandValidatorTests
    {
        private readonly UpdateCoursePrerequisitesCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new UpdateCoursePrerequisitesCommand(Guid.NewGuid(), ["Basic programming", "OOP"]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var command = new UpdateCoursePrerequisitesCommand(Guid.Empty, ["Basic programming"]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_NullCollection_ReturnsValidationError()
        {
            var command = new UpdateCoursePrerequisitesCommand(Guid.NewGuid(), null!);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Prerequisites");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_EmptyPrerequisite_ReturnsValidationError(string prerequisite)
        {
            var command = new UpdateCoursePrerequisitesCommand(Guid.NewGuid(), [prerequisite]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.required");
        }

        [Fact]
        public void Validate_PrerequisiteTooLong_ReturnsValidationError()
        {
            var longPrerequisite = new string('A', 501);
            var command = new UpdateCoursePrerequisitesCommand(Guid.NewGuid(), [longPrerequisite]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }
    }
}
