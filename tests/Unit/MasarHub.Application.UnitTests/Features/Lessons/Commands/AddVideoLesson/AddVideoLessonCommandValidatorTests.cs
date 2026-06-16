using FluentAssertions;
using MasarHub.Application.Features.Lessons.Commands.AddVideoLesson;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.AddVideoLesson
{
    [Trait("UnitTests.Feature.Lessons", "AddVideoLesson")]
    public sealed class AddVideoLessonCommandValidatorTests
    {
        private readonly AddVideoLessonCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new AddVideoLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, "Introduction to Programming", "Basic description", "video_public_key_123");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("Short")]
        public void Validate_InvalidTitle_ReturnsValidationError(string title)
        {
            var command = new AddVideoLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, title, null, "video_public_key_123");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_TitleTooLong_ReturnsValidationError()
        {
            var command = new AddVideoLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, new string('A', 201), null, "video_public_key_123");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Fact]
        public void Validate_DescriptionTooShort_ReturnsValidationError()
        {
            var command = new AddVideoLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, "Introduction to Programming", "Short", "video_public_key_123");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.min_length");
        }

        [Fact]
        public void Validate_DescriptionTooLong_ReturnsValidationError()
        {
            var command = new AddVideoLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, "Introduction to Programming", new string('A', 1001), "video_public_key_123");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ShortKey")]
        public void Validate_InvalidFileKey_ReturnsValidationError(string fileKey)
        {
            var command = new AddVideoLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, "Introduction to Programming", null, fileKey);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "FileKey");
        }

        [Fact]
        public void Validate_FileKeyTooLong_ReturnsValidationError()
        {
            var command = new AddVideoLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, "Introduction to Programming", null, new string('K', 201));

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }
    }
}
