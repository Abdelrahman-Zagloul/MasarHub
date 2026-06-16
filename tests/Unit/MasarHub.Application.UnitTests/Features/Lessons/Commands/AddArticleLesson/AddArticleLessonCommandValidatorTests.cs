using FluentAssertions;
using MasarHub.Application.Features.Lessons.Commands.AddArticleLesson;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.AddArticleLesson
{
    [Trait("UnitTests.Feature.Lessons", "AddArticleLesson")]
    public sealed class AddArticleLessonCommandValidatorTests
    {
        private readonly AddArticleLessonCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new AddArticleLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, "Introduction to Programming", "Article content here", "Optional description");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("Short")]
        public void Validate_InvalidTitle_ReturnsValidationError(string title)
        {
            var command = new AddArticleLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, title, "Content here", null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_TitleTooLong_ReturnsValidationError()
        {
            var command = new AddArticleLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, new string('A', 201), "Content here", null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("Short")]
        public void Validate_InvalidContent_ReturnsValidationError(string content)
        {
            var command = new AddArticleLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, "Introduction to Programming", content, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Content");
        }

        [Fact]
        public void Validate_DescriptionTooShort_ReturnsValidationError()
        {
            var command = new AddArticleLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, "Introduction to Programming", "Content here", "Short");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.min_length");
        }

        [Fact]
        public void Validate_DescriptionTooLong_ReturnsValidationError()
        {
            var command = new AddArticleLessonCommand(Guid.NewGuid(), Guid.NewGuid(), false, "Introduction to Programming", "Content here", new string('A', 1001));

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }
    }
}
