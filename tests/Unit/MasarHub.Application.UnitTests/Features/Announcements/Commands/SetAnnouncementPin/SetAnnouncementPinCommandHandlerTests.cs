using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Announcements.Commands.SetAnnouncementPin;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Announcements.Commands.SetAnnouncementPin
{
    [Trait("UnitTests.Feature.Announcements", "SetAnnouncementPin")]
    public sealed class SetAnnouncementPinCommandHandlerTests
    {
        private readonly Mock<ICourseQuery> _courseQueryMock;
        private readonly Mock<IRepository<CourseAnnouncement>> _announcementRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly SetAnnouncementPinCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();
        private static readonly Guid CourseId = Guid.NewGuid();

        public SetAnnouncementPinCommandHandlerTests()
        {
            _courseQueryMock = new Mock<ICourseQuery>();
            _announcementRepositoryMock = new Mock<IRepository<CourseAnnouncement>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new SetAnnouncementPinCommandHandler(
                _courseQueryMock.Object,
                _announcementRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFoundError()
        {
            var command = new SetAnnouncementPinCommand(CourseId, Guid.NewGuid(), InstructorId, true);

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
            var command = new SetAnnouncementPinCommand(CourseId, Guid.NewGuid(), InstructorId, true);

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
            var command = new SetAnnouncementPinCommand(CourseId, announcementId, InstructorId, true);

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
        public async Task Handle_PinTrue_PinsAnnouncement()
        {
            var announcementId = Guid.NewGuid();
            var command = new SetAnnouncementPinCommand(CourseId, announcementId, InstructorId, true);
            var announcement = CourseAnnouncement.Create(CourseId, InstructorId, "Title", "Content", AnnouncementImportance.Normal).Value;

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));

            _announcementRepositoryMock
                .Setup(x => x.GetByIdAsync(announcementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(announcement);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            announcement.IsPinned.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PinFalse_UnpinsAnnouncement()
        {
            var announcementId = Guid.NewGuid();
            var command = new SetAnnouncementPinCommand(CourseId, announcementId, InstructorId, false);
            var announcement = CourseAnnouncement.Create(CourseId, InstructorId, "Title", "Content", AnnouncementImportance.Normal).Value;
            announcement.Pin();

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));

            _announcementRepositoryMock
                .Setup(x => x.GetByIdAsync(announcementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(announcement);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            announcement.IsPinned.Should().BeFalse();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
