using FluentAssertions;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent;

namespace MasarHub.Application.UnitTests.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent
{
    [Trait("UnitTests.Feature.Announcements", "GetCourseAnnouncementByIdForStudent")]
    public sealed class GetCourseAnnouncementByIdForStudentQueryValidatorTests
    {
        private readonly GetCourseAnnouncementByIdForStudentQueryValidator _sut = new();

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = new GetCourseAnnouncementByIdForStudentQuery(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var query = new GetCourseAnnouncementByIdForStudentQuery(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyAnnouncementId_ReturnsError()
        {
            var query = new GetCourseAnnouncementByIdForStudentQuery(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "AnnouncementId");
        }

        [Fact]
        public void Validate_EmptyStudentId_ReturnsError()
        {
            var query = new GetCourseAnnouncementByIdForStudentQuery(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "StudentId");
        }
    }
}
