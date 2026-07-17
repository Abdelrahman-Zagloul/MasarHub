using FluentAssertions;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor;

namespace MasarHub.Application.UnitTests.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor
{
    [Trait("UnitTests.Feature.Announcements", "GetCourseAnnouncementByIdForInstructor")]
    public sealed class GetCourseAnnouncementByIdForInstructorQueryValidatorTests
    {
        private readonly GetCourseAnnouncementByIdForInstructorQueryValidator _sut = new();

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = new GetCourseAnnouncementByIdForInstructorQuery(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var query = new GetCourseAnnouncementByIdForInstructorQuery(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyAnnouncementId_ReturnsError()
        {
            var query = new GetCourseAnnouncementByIdForInstructorQuery(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "AnnouncementId");
        }

        [Fact]
        public void Validate_EmptyInstructorId_ReturnsError()
        {
            var query = new GetCourseAnnouncementByIdForInstructorQuery(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "InstructorId");
        }
    }
}
