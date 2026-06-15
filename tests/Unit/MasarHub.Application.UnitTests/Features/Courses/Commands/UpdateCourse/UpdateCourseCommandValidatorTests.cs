using FluentAssertions;
using MasarHub.Application.Features.Courses.Commands.UpdateCourse;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.UpdateCourse
{
    [Trait("UnitTests.Feature.Courses", "UpdateCourse")]
    public sealed class UpdateCourseCommandValidatorTests
    {
        private readonly UpdateCourseCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new UpdateCourseCommand(Guid.NewGuid(), "New Title", null, null, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var command = new UpdateCourseCommand(Guid.Empty, "New Title", null, null, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_NoUpdates_ReturnsValidationError()
        {
            var command = new UpdateCourseCommand(Guid.NewGuid(), null, null, null, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.at_least_one_field_required");
        }

        [Fact]
        public void Validate_InvalidLanguage_ReturnsValidationError()
        {
            var command = new UpdateCourseCommand(Guid.NewGuid(), null, null, null, (CourseLanguage)99, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Language");
        }

        [Fact]
        public void Validate_InvalidLevel_ReturnsValidationError()
        {
            var command = new UpdateCourseCommand(Guid.NewGuid(), null, null, null, null, (CourseLevel)99, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Level");
        }
    }
}
