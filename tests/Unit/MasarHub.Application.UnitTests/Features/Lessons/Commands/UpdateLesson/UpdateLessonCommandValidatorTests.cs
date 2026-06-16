using FluentAssertions;
using MasarHub.Application.Features.Lessons.Commands.UpdateLesson;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.UpdateLesson
{
    [Trait("UnitTests.Feature.Lessons", "UpdateLesson")]
    public sealed class UpdateLessonCommandValidatorTests
    {
        private readonly UpdateLessonCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommandWithTitle_ReturnsNoErrors()
        {
            var command = new UpdateLessonCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Updated Title", null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ValidCommandWithDescription_ReturnsNoErrors()
        {
            var command = new UpdateLessonCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, "Updated Description");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_NoTitleAndNoDescription_ReturnsValidationError(string? title)
        {
            var command = new UpdateLessonCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), title, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_TitleTooShort_ReturnsValidationError()
        {
            var command = new UpdateLessonCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Short", null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.min_length");
        }

        [Fact]
        public void Validate_TitleTooLong_ReturnsValidationError()
        {
            var command = new UpdateLessonCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new string('A', 201), null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Fact]
        public void Validate_DescriptionTooLong_ReturnsValidationError()
        {
            var command = new UpdateLessonCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, new string('A', 2001));

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }
    }
}
