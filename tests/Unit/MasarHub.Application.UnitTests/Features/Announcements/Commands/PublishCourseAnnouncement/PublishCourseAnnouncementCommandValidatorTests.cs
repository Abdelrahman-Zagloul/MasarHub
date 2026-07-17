using FluentAssertions;
using MasarHub.Application.Features.Announcements.Commands.PublishCourseAnnouncement;

namespace MasarHub.Application.UnitTests.Features.Announcements.Commands.PublishCourseAnnouncement
{
    [Trait("UnitTests.Feature.Announcements", "PublishCourseAnnouncement")]
    public sealed class PublishCourseAnnouncementCommandValidatorTests
    {
        private readonly PublishCourseAnnouncementCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new PublishCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var command = new PublishCourseAnnouncementCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyAnnouncementId_ReturnsError()
        {
            var command = new PublishCourseAnnouncementCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "AnnouncementId");
        }
    }
}
