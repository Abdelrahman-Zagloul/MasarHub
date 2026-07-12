using FluentAssertions;
using MasarHub.Application.Features.Announcements.Commands.DeleteCourseAnnouncement;

namespace MasarHub.Application.UnitTests.Features.Announcements.Commands.DeleteCourseAnnouncement
{
    [Trait("UnitTests.Feature.Announcements", "DeleteCourseAnnouncement")]
    public sealed class DeleteCourseAnnouncementCommandValidatorTests
    {
        private readonly DeleteCourseAnnouncementCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new DeleteCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var command = new DeleteCourseAnnouncementCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyAnnouncementId_ReturnsError()
        {
            var command = new DeleteCourseAnnouncementCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "AnnouncementId");
        }
    }
}
