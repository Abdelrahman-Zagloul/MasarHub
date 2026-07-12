using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor
{
    [Trait("UnitTests.Feature.Announcements", "GetCourseAnnouncementByIdForInstructor")]
    public sealed class GetCourseAnnouncementByIdForInstructorQueryHandlerTests
    {
        private readonly Mock<IAnnouncementQuery> _announcementQueryMock;
        private readonly GetCourseAnnouncementByIdForInstructorQueryHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();
        private static readonly Guid CourseId = Guid.NewGuid();

        public GetCourseAnnouncementByIdForInstructorQueryHandlerTests()
        {
            _announcementQueryMock = new Mock<IAnnouncementQuery>();
            _sut = new GetCourseAnnouncementByIdForInstructorQueryHandler(_announcementQueryMock.Object);
        }

        [Fact]
        public async Task Handle_AnnouncementFound_ReturnsAnnouncement()
        {
            var announcementId = Guid.NewGuid();
            var response = new InstructorCourseAnnouncementResponse(
                announcementId, CourseId, InstructorId, "Title", "Content", false, null, null, null,
                AnnouncementImportance.Normal, false, DateTimeOffset.UtcNow, null);

            _announcementQueryMock
                .Setup(x => x.GetByIdForInstructorAsync(CourseId, announcementId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.Handle(new GetCourseAnnouncementByIdForInstructorQuery(CourseId, announcementId, InstructorId), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(response);
        }

        [Fact]
        public async Task Handle_NotFound_ReturnsNotFoundError()
        {
            _announcementQueryMock
                .Setup(x => x.GetByIdForInstructorAsync(CourseId, It.IsAny<Guid>(), InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((InstructorCourseAnnouncementResponse?)null);

            var result = await _sut.Handle(new GetCourseAnnouncementByIdForInstructorQuery(CourseId, Guid.NewGuid(), InstructorId), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course_announcement.not_found");
        }
    }
}
