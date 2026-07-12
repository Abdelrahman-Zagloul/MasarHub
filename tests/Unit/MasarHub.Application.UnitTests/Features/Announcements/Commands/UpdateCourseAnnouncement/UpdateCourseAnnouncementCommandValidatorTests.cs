using FluentAssertions;
using MasarHub.Application.Features.Announcements.Commands.UpdateCourseAnnouncement;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.UnitTests.Features.Announcements.Commands.UpdateCourseAnnouncement
{
    [Trait("UnitTests.Feature.Announcements", "UpdateCourseAnnouncement")]
    public sealed class UpdateCourseAnnouncementCommandValidatorTests
    {
        private readonly UpdateCourseAnnouncementCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Valid Title", "Valid Content", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_OnlyTitleProvided_ReturnsNoErrors()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Valid Title", null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_OnlyContentProvided_ReturnsNoErrors()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, "Valid Content", null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_OnlyImportanceProvided_ReturnsNoErrors()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, null, AnnouncementImportance.High);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_AllFieldsNull_ReturnsAtLeastOneError()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.at_least_one");
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), "Valid Title", "Valid Content", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyAnnouncementId_ReturnsError()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), "Valid Title", "Valid Content", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "AnnouncementId");
        }

        [Fact]
        public void Validate_EmptyTitle_ReturnsError()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "", "Valid Content", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_TitleExceedsMaxLength_ReturnsError()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new string('x', 201), "Valid Content", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_TitleTooShort_ReturnsError()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "AB", "Valid Content", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_EmptyContent_ReturnsError()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Valid Title", "", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Content");
        }

        [Fact]
        public void Validate_ContentExceedsMaxLength_ReturnsError()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Valid Title", new string('x', 4001), AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Content");
        }

        [Fact]
        public void Validate_ContentTooShort_ReturnsError()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Valid Title", "AB", AnnouncementImportance.Normal);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Content");
        }

        [Fact]
        public void Validate_InvalidImportance_ReturnsError()
        {
            var command = new UpdateCourseAnnouncementCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Valid Title", "Valid Content", (AnnouncementImportance)99);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Importance");
        }
    }
}
