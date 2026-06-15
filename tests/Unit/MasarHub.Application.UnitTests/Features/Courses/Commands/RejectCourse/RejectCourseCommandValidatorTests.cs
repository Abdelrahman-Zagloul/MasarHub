using FluentAssertions;
using MasarHub.Application.Features.Courses.Commands.RejectCourse;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.RejectCourse
{
    [Trait("UnitTests.Feature.Courses", "RejectCourse")]
    public sealed class RejectCourseCommandValidatorTests
    {
        private readonly RejectCourseCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new RejectCourseCommand(Guid.NewGuid(), Guid.NewGuid(), "Insufficient content quality");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var command = new RejectCourseCommand(Guid.Empty, Guid.NewGuid(), "Some reason");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_EmptyReason_ReturnsValidationError(string reason)
        {
            var command = new RejectCourseCommand(Guid.NewGuid(), Guid.NewGuid(), reason);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Reason");
        }

        [Fact]
        public void Validate_ReasonTooShort_ReturnsValidationError()
        {
            var command = new RejectCourseCommand(Guid.NewGuid(), Guid.NewGuid(), "AB");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.min_length");
        }

        [Fact]
        public void Validate_ReasonTooLong_ReturnsValidationError()
        {
            var longReason = new string('A', 1001);
            var command = new RejectCourseCommand(Guid.NewGuid(), Guid.NewGuid(), longReason);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }
    }
}
