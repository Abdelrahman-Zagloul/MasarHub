using FluentAssertions;
using MasarHub.Application.Features.Exams.Commands.ToggleExamPublished;

namespace MasarHub.Application.UnitTests.Features.Exams.Commands.ToggleExamPublished
{
    [Trait("UnitTests.Feature.Exams", "ToggleExamPublished")]
    public sealed class ToggleExamPublishedCommandValidatorTests
    {
        private readonly ToggleExamPublishedCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new ToggleExamPublishedCommand(Guid.NewGuid(), Guid.NewGuid(), true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyExamId_ReturnsValidationError()
        {
            var command = new ToggleExamPublishedCommand(Guid.Empty, Guid.NewGuid(), true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ExamId");
        }

        [Fact]
        public void Validate_EmptyInstructorId_ReturnsValidationError()
        {
            var command = new ToggleExamPublishedCommand(Guid.NewGuid(), Guid.Empty, true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "InstructorId");
        }
    }
}