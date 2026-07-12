using FluentAssertions;
using MasarHub.Application.Features.Announcements.Commands.SetAnnouncementPin;

namespace MasarHub.Application.UnitTests.Features.Announcements.Commands.SetAnnouncementPin
{
    [Trait("UnitTests.Feature.Announcements", "SetAnnouncementPin")]
    public sealed class SetAnnouncementPinCommandValidatorTests
    {
        private readonly SetAnnouncementPinCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new SetAnnouncementPinCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var command = new SetAnnouncementPinCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyAnnouncementId_ReturnsError()
        {
            var command = new SetAnnouncementPinCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "AnnouncementId");
        }
    }
}
