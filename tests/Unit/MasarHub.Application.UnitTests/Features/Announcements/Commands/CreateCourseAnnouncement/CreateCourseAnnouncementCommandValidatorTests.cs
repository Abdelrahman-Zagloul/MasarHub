using FluentAssertions;
using MasarHub.Application.Features.Announcements.Commands.CreateCourseAnnouncement;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.UnitTests.Features.Announcements.Commands.CreateCourseAnnouncement
{
    [Trait("UnitTests.Feature.Announcements", "CreateCourseAnnouncement")]
    public sealed class CreateCourseAnnouncementCommandValidatorTests
    {
        private readonly CreateCourseAnnouncementCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new CreateCourseAnnouncementCommand(
                Guid.NewGuid(), Guid.NewGuid(), "Valid Title", "Valid Content", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var command = new CreateCourseAnnouncementCommand(
                Guid.Empty, Guid.NewGuid(), "Valid Title", "Valid Content", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyTitle_ReturnsError()
        {
            var command = new CreateCourseAnnouncementCommand(
                Guid.NewGuid(), Guid.NewGuid(), "", "Valid Content", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_TitleExceedsMaxLength_ReturnsError()
        {
            var command = new CreateCourseAnnouncementCommand(
                Guid.NewGuid(), Guid.NewGuid(), new string('x', 201), "Valid Content", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_EmptyContent_ReturnsError()
        {
            var command = new CreateCourseAnnouncementCommand(
                Guid.NewGuid(), Guid.NewGuid(), "Valid Title", "", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Content");
        }

        [Fact]
        public void Validate_ContentExceedsMaxLength_ReturnsError()
        {
            var command = new CreateCourseAnnouncementCommand(
                Guid.NewGuid(), Guid.NewGuid(), "Valid Title", new string('x', 4001), AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Content");
        }

        [Fact]
        public void Validate_InvalidImportance_ReturnsError()
        {
            var command = new CreateCourseAnnouncementCommand(
                Guid.NewGuid(), Guid.NewGuid(), "Valid Title", "Valid Content", (AnnouncementImportance)99);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Importance");
        }
    }
}
