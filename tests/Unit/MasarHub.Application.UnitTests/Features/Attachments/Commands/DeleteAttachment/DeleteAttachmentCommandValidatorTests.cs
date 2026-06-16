using FluentAssertions;
using MasarHub.Application.Features.Attachments.Commands.DeleteAttachment;

namespace MasarHub.Application.UnitTests.Features.Attachments.Commands.DeleteAttachment
{
    [Trait("UnitTests.Feature.Attachments", "DeleteAttachment")]
    public sealed class DeleteAttachmentCommandValidatorTests
    {
        private readonly DeleteAttachmentCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new DeleteAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyLessonId_ReturnsValidationError()
        {
            var command = new DeleteAttachmentCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "LessonId");
        }

        [Fact]
        public void Validate_EmptyAttachmentId_ReturnsValidationError()
        {
            var command = new DeleteAttachmentCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "AttachmentId");
        }

        [Fact]
        public void Validate_EmptyInstructorId_ReturnsValidationError()
        {
            var command = new DeleteAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "InstructorId");
        }
    }
}
