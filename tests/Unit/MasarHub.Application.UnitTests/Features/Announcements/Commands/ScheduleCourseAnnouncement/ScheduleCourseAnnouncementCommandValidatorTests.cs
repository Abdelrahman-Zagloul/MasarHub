using FluentAssertions;
using MasarHub.Application.Features.Announcements.Commands.ScheduleCourseAnnouncement;

namespace MasarHub.Application.UnitTests.Features.Announcements.Commands.ScheduleCourseAnnouncement
{
    [Trait("UnitTests.Feature.Announcements", "ScheduleCourseAnnouncement")]
    public sealed class ScheduleCourseAnnouncementCommandValidatorTests
    {
        private readonly ScheduleCourseAnnouncementCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new ScheduleCourseAnnouncementCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(7));

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var command = new ScheduleCourseAnnouncementCommand(
                Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(7));

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyAnnouncementId_ReturnsError()
        {
            var command = new ScheduleCourseAnnouncementCommand(
                Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(7));

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "AnnouncementId");
        }

        [Fact]
        public void Validate_ScheduledAtInPast_ReturnsError()
        {
            var command = new ScheduleCourseAnnouncementCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(-1));

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "course_announcement.invalid_schedule_time");
        }

        [Fact]
        public void Validate_ScheduledAtNow_ReturnsError()
        {
            var command = new ScheduleCourseAnnouncementCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "course_announcement.invalid_schedule_time");
        }
    }
}
