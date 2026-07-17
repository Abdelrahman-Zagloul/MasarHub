using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent
{
    [Trait("UnitTests.Feature.Announcements", "GetCourseAnnouncementByIdForStudent")]
    public sealed class GetCourseAnnouncementByIdForStudentQueryHandlerTests
    {
        private readonly Mock<IAnnouncementQuery> _announcementQueryMock;
        private readonly GetCourseAnnouncementByIdForStudentQueryHandler _sut;
        private static readonly Guid CourseId = Guid.NewGuid();
        private static readonly Guid StudentId = Guid.NewGuid();

        public GetCourseAnnouncementByIdForStudentQueryHandlerTests()
        {
            _announcementQueryMock = new Mock<IAnnouncementQuery>();
            _sut = new GetCourseAnnouncementByIdForStudentQueryHandler(_announcementQueryMock.Object);
        }

        [Fact]
        public async Task Handle_EnrolledAndFound_ReturnsAnnouncement()
        {
            var announcementId = Guid.NewGuid();
            var response = new StudentCourseAnnouncementResponse(
                announcementId, CourseId, "Title", "Content", DateTimeOffset.UtcNow,
                AnnouncementImportance.Normal, false, DateTimeOffset.UtcNow);

            _announcementQueryMock
                .Setup(x => x.GetByIdForStudentAsync(CourseId, announcementId, StudentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StudentAnnouncementResult(true, response));

            var result = await _sut.Handle(new GetCourseAnnouncementByIdForStudentQuery(CourseId, announcementId, StudentId), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(response);
        }

        [Fact]
        public async Task Handle_NotEnrolled_ReturnsForbiddenError()
        {
            _announcementQueryMock
                .Setup(x => x.GetByIdForStudentAsync(CourseId, It.IsAny<Guid>(), StudentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StudentAnnouncementResult(false, null));

            var result = await _sut.Handle(new GetCourseAnnouncementByIdForStudentQuery(CourseId, Guid.NewGuid(), StudentId), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_enrolled");
        }

        [Fact]
        public async Task Handle_EnrolledNotFound_ReturnsNotFoundError()
        {
            _announcementQueryMock
                .Setup(x => x.GetByIdForStudentAsync(CourseId, It.IsAny<Guid>(), StudentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StudentAnnouncementResult(true, null));

            var result = await _sut.Handle(new GetCourseAnnouncementByIdForStudentQuery(CourseId, Guid.NewGuid(), StudentId), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course_announcement.not_found");
        }
    }
}
