using FluentAssertions;
using MasarHub.Application.Features.Courses.Commands.CreateCourse;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.CreateCourse
{
    [Trait("UnitTests.Feature.Courses", "CreateCourse")]
    public sealed class CreateCourseCommandValidatorTests
    {
        private readonly CreateCourseCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new CreateCourseCommand("Programming 101", "Learn programming basics", 49.99m, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_EmptyTitle_ReturnsValidationError(string title)
        {
            var command = new CreateCourseCommand(title, "Description", 0, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_TitleTooShort_ReturnsValidationError()
        {
            var command = new CreateCourseCommand("AB", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.min_length");
        }

        [Fact]
        public void Validate_TitleTooLong_ReturnsValidationError()
        {
            var longTitle = new string('A', 201);
            var command = new CreateCourseCommand(longTitle, "Description", 0, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_EmptyDescription_ReturnsValidationError(string description)
        {
            var command = new CreateCourseCommand("Programming 101", description, 0, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Description");
        }

        [Fact]
        public void Validate_DescriptionTooShort_ReturnsValidationError()
        {
            var command = new CreateCourseCommand("Programming 101", "Short", 0, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.min_length");
        }

        [Fact]
        public void Validate_DescriptionTooLong_ReturnsValidationError()
        {
            var longDesc = new string('A', 2001);
            var command = new CreateCourseCommand("Programming 101", longDesc, 0, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Fact]
        public void Validate_NegativePrice_ReturnsValidationError()
        {
            var command = new CreateCourseCommand("Programming 101", "Description", -10, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Price");
        }

        [Fact]
        public void Validate_EmptyCategoryId_ReturnsValidationError()
        {
            var command = new CreateCourseCommand("Programming 101", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, Guid.Empty);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
        }

        [Fact]
        public void Validate_InvalidLanguage_ReturnsValidationError()
        {
            var command = new CreateCourseCommand("Programming 101", "Description", 0, (CourseLanguage)99, CourseLevel.Beginner, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Language");
        }

        [Fact]
        public void Validate_InvalidLevel_ReturnsValidationError()
        {
            var command = new CreateCourseCommand("Programming 101", "Description", 0, CourseLanguage.English, (CourseLevel)99, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Level");
        }
    }
}
