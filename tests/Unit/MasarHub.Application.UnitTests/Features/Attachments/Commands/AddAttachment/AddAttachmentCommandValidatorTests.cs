using FluentAssertions;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Features.Attachments.Commands.AddAttachment;

namespace MasarHub.Application.UnitTests.Features.Attachments.Commands.AddAttachment
{
    [Trait("UnitTests.Feature.Attachments", "AddAttachment")]
    public sealed class AddAttachmentCommandValidatorTests
    {
        private readonly AddAttachmentCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new AddAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), CreateFileResource());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyLessonId_ReturnsValidationError()
        {
            var command = new AddAttachmentCommand(Guid.Empty, Guid.NewGuid(), CreateFileResource());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "LessonId");
        }

        [Fact]
        public void Validate_EmptyInstructorId_ReturnsValidationError()
        {
            var command = new AddAttachmentCommand(Guid.NewGuid(), Guid.Empty, CreateFileResource());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "InstructorId");
        }

        private static FileResource CreateFileResource()
        {
            return new FileResource("file.pdf", "application/pdf", new MemoryStream(), 1024);
        }
    }
}
