using FluentAssertions;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Features.Lessons.Commands.UpdateVideoThumbnail;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.UpdateVideoThumbnail
{
    [Trait("UnitTests.Feature.Lessons", "UpdateVideoThumbnail")]
    public sealed class UpdateVideoThumbnailCommandValidatorTests
    {
        private readonly UpdateVideoThumbnailCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new UpdateVideoThumbnailCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new FileResource("test.jpg", "image/jpeg", new MemoryStream(), 1024));

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyModuleId_ReturnsValidationError()
        {
            var command = new UpdateVideoThumbnailCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), new FileResource("test.jpg", "image/jpeg", new MemoryStream(), 1024));

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ModuleId");
        }

        [Fact]
        public void Validate_EmptyLessonId_ReturnsValidationError()
        {
            var command = new UpdateVideoThumbnailCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), new FileResource("test.jpg", "image/jpeg", new MemoryStream(), 1024));

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "LessonId");
        }
    }
}
