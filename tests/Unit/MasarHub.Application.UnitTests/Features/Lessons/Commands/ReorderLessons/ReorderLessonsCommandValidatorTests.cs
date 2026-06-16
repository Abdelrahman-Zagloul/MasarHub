using FluentAssertions;
using MasarHub.Application.Features.Lessons.Commands.ReorderLessons;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.ReorderLessons
{
    [Trait("UnitTests.Feature.Lessons", "ReorderLessons")]
    public sealed class ReorderLessonsCommandValidatorTests
    {
        private readonly ReorderLessonsCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new ReorderLessonsCommand(Guid.NewGuid(), Guid.NewGuid(), [Guid.NewGuid()]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyModuleId_ReturnsValidationError()
        {
            var command = new ReorderLessonsCommand(Guid.Empty, Guid.NewGuid(), [Guid.NewGuid()]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ModuleId");
        }

        [Fact]
        public void Validate_EmptyOrderedLessonIds_ReturnsValidationError()
        {
            var command = new ReorderLessonsCommand(Guid.NewGuid(), Guid.NewGuid(), []);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "OrderedLessonIds");
        }

        [Fact]
        public void Validate_DuplicateLessonIds_ReturnsValidationError()
        {
            var id = Guid.NewGuid();
            var command = new ReorderLessonsCommand(Guid.NewGuid(), Guid.NewGuid(), [id, id]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "OrderedLessonIds");
        }

        [Fact]
        public void Validate_EmptyGuidInOrderedLessonIds_ReturnsValidationError()
        {
            var command = new ReorderLessonsCommand(Guid.NewGuid(), Guid.NewGuid(), [Guid.NewGuid(), Guid.Empty]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "OrderedLessonIds");
        }
    }
}
