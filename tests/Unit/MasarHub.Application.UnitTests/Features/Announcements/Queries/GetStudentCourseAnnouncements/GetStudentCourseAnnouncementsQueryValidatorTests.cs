using FluentAssertions;
using MasarHub.Application.Features.Announcements.Queries.GetStudentCourseAnnouncements;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.UnitTests.Features.Announcements.Queries.GetStudentCourseAnnouncements
{
    [Trait("UnitTests.Feature.Announcements", "GetStudentCourseAnnouncements")]
    public sealed class GetStudentCourseAnnouncementsQueryValidatorTests
    {
        private readonly GetStudentCourseAnnouncementsQueryValidator _sut = new();

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = new GetStudentCourseAnnouncementsQuery(Guid.NewGuid(), Guid.NewGuid(), AnnouncementImportance.High, false, "search", 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var query = new GetStudentCourseAnnouncementsQuery(Guid.Empty, Guid.NewGuid(), null, null, null, 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyStudentId_ReturnsError()
        {
            var query = new GetStudentCourseAnnouncementsQuery(Guid.NewGuid(), Guid.Empty, null, null, null, 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "StudentId");
        }

        [Fact]
        public void Validate_InvalidImportance_ReturnsError()
        {
            var query = new GetStudentCourseAnnouncementsQuery(Guid.NewGuid(), Guid.NewGuid(), (AnnouncementImportance)999, null, null, 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Importance");
        }

        [Fact]
        public void Validate_InvalidPageNumber_ReturnsError()
        {
            var query = new GetStudentCourseAnnouncementsQuery(Guid.NewGuid(), Guid.NewGuid(), null, null, null, 0, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PageNumber");
        }

        [Fact]
        public void Validate_InvalidPageSize_ReturnsError()
        {
            var query = new GetStudentCourseAnnouncementsQuery(Guid.NewGuid(), Guid.NewGuid(), null, null, null, 1, 0);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
        }

        [Fact]
        public void Validate_PageSizeTooLarge_ReturnsError()
        {
            var query = new GetStudentCourseAnnouncementsQuery(Guid.NewGuid(), Guid.NewGuid(), null, null, null, 1, 100);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
        }

        [Fact]
        public void Validate_SearchTooLong_ReturnsError()
        {
            var query = new GetStudentCourseAnnouncementsQuery(Guid.NewGuid(), Guid.NewGuid(), null, null, new string('a', 201), 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Search");
        }
    }
}
