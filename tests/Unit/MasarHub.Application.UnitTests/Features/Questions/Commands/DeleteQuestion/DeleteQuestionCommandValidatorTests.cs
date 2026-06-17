using FluentAssertions;
using MasarHub.Application.Features.Questions.Commands.DeleteQuestion;

namespace MasarHub.Application.UnitTests.Features.Questions.Commands.DeleteQuestion
{
    [Trait("UnitTests.Feature.Questions", "DeleteQuestion")]
    public sealed class DeleteQuestionCommandValidatorTests
    {
        private readonly DeleteQuestionCommandValidator _sut = new();
        private static readonly Guid InstructorId = Guid.NewGuid();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new DeleteQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyExamId_ReturnsValidationError()
        {
            var command = new DeleteQuestionCommand(Guid.Empty, Guid.NewGuid(), InstructorId);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ExamId");
        }

        [Fact]
        public void Validate_EmptyQuestionId_ReturnsValidationError()
        {
            var command = new DeleteQuestionCommand(Guid.NewGuid(), Guid.Empty, InstructorId);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "QuestionId");
        }
    }
}
