using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Announcements.Commands.UpdateCourseAnnouncement;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Announcements.Commands.UpdateCourseAnnouncement
{
    [Trait("UnitTests.Feature.Announcements", "UpdateCourseAnnouncement")]
    public sealed class UpdateCourseAnnouncementCommandHandlerTests
    {
        private readonly Mock<ICourseQuery> _courseQueryMock;
        private readonly Mock<IRepository<CourseAnnouncement>> _announcementRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateCourseAnnouncementCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();
        private static readonly Guid CourseId = Guid.NewGuid();

        public UpdateCourseAnnouncementCommandHandlerTests()
        {
            _courseQueryMock = new Mock<ICourseQuery>();
            _announcementRepositoryMock = new Mock<IRepository<CourseAnnouncement>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new UpdateCourseAnnouncementCommandHandler(
                _courseQueryMock.Object,
                _announcementRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFoundError()
        {
            var command = new UpdateCourseAnnouncementCommand(CourseId, Guid.NewGuid(), InstructorId, "New Title", "New Content", AnnouncementImportance.High);

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InstructorNotOwner_ReturnsForbiddenError()
        {
            var command = new UpdateCourseAnnouncementCommand(CourseId, Guid.NewGuid(), InstructorId, "New Title", "New Content", AnnouncementImportance.High);

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AnnouncementNotFound_ReturnsNotFoundError()
        {
            var announcementId = Guid.NewGuid();
            var command = new UpdateCourseAnnouncementCommand(CourseId, announcementId, InstructorId, "New Title", "New Content", AnnouncementImportance.High);

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));

            _announcementRepositoryMock
                .Setup(x => x.GetByIdAsync(announcementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseAnnouncement?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course_announcement.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidRequest_UpdatesAnnouncement()
        {
            var announcementId = Guid.NewGuid();
            var command = new UpdateCourseAnnouncementCommand(CourseId, announcementId, InstructorId, "Updated Title", "Updated Content", AnnouncementImportance.High);
            var announcement = CourseAnnouncement.Create(CourseId, InstructorId, "Old Title", "Old Content", AnnouncementImportance.Normal).Value;

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));

            _announcementRepositoryMock
                .Setup(x => x.GetByIdAsync(announcementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(announcement);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            announcement.Title.Should().Be("Updated Title");
            announcement.Content.Should().Be("Updated Content");
            announcement.Importance.Should().Be(AnnouncementImportance.High);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PublishedAnnouncement_ReturnsCannotEditError()
        {
            var announcementId = Guid.NewGuid();
            var command = new UpdateCourseAnnouncementCommand(CourseId, announcementId, InstructorId, "New Title", "New Content", AnnouncementImportance.High);
            var announcement = CourseAnnouncement.Create(CourseId, InstructorId, "Title", "Content", AnnouncementImportance.Normal).Value;
            announcement.Publish();

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));

            _announcementRepositoryMock
                .Setup(x => x.GetByIdAsync(announcementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(announcement);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course_announcement.cannot_edit_after_publish");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
