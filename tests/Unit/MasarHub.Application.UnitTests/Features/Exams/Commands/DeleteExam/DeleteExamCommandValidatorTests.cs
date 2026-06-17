using FluentAssertions;
using MasarHub.Application.Features.Exams.Commands.DeleteExam;

namespace MasarHub.Application.UnitTests.Features.Exams.Commands.DeleteExam
{
    [Trait("UnitTests.Feature.Exams", "DeleteExam")]
    public sealed class DeleteExamCommandValidatorTests
    {
        private readonly DeleteExamCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new DeleteExamCommand(Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyExamId_ReturnsValidationError()
        {
            var command = new DeleteExamCommand(Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ExamId");
        }

        [Fact]
        public void Validate_EmptyInstructorId_ReturnsValidationError()
        {
            var command = new DeleteExamCommand(Guid.NewGuid(), Guid.Empty);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "InstructorId");
        }
    }
}
