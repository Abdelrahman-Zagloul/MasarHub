using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Announcements.Commands.ScheduleCourseAnnouncement;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Announcements.Commands.ScheduleCourseAnnouncement
{
    [Trait("UnitTests.Feature.Announcements", "ScheduleCourseAnnouncement")]
    public sealed class ScheduleCourseAnnouncementCommandHandlerTests
    {
        private readonly Mock<ICourseQuery> _courseQueryMock;
        private readonly Mock<IRepository<CourseAnnouncement>> _announcementRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ScheduleCourseAnnouncementCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();
        private static readonly Guid CourseId = Guid.NewGuid();

        public ScheduleCourseAnnouncementCommandHandlerTests()
        {
            _courseQueryMock = new Mock<ICourseQuery>();
            _announcementRepositoryMock = new Mock<IRepository<CourseAnnouncement>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new ScheduleCourseAnnouncementCommandHandler(
                _courseQueryMock.Object,
                _announcementRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFoundError()
        {
            var command = new ScheduleCourseAnnouncementCommand(CourseId, Guid.NewGuid(), InstructorId, DateTimeOffset.UtcNow.AddDays(1));

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
            var command = new ScheduleCourseAnnouncementCommand(CourseId, Guid.NewGuid(), InstructorId, DateTimeOffset.UtcNow.AddDays(1));

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
            var command = new ScheduleCourseAnnouncementCommand(CourseId, announcementId, InstructorId, DateTimeOffset.UtcNow.AddDays(1));

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
        public async Task Handle_AlreadyScheduled_ReturnsAlreadyScheduledError()
        {
            var announcementId = Guid.NewGuid();
            var command = new ScheduleCourseAnnouncementCommand(CourseId, announcementId, InstructorId, DateTimeOffset.UtcNow.AddDays(7));
            var announcement = CourseAnnouncement.Create(CourseId, InstructorId, "Title", "Content", AnnouncementImportance.Normal).Value;
            announcement.Schedule(DateTimeOffset.UtcNow.AddDays(1));

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));

            _announcementRepositoryMock
                .Setup(x => x.GetByIdAsync(announcementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(announcement);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course_announcement.already_scheduled");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ScheduleInPast_ReturnsDomainError()
        {
            var announcementId = Guid.NewGuid();
            var command = new ScheduleCourseAnnouncementCommand(CourseId, announcementId, InstructorId, DateTimeOffset.UtcNow.AddDays(-1));
            var announcement = CourseAnnouncement.Create(CourseId, InstructorId, "Title", "Content", AnnouncementImportance.Normal).Value;

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));

            _announcementRepositoryMock
                .Setup(x => x.GetByIdAsync(announcementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(announcement);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course_announcement.invalid_schedule_time");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidRequest_SchedulesAnnouncement()
        {
            var announcementId = Guid.NewGuid();
            var scheduledAt = DateTimeOffset.UtcNow.AddDays(7);
            var command = new ScheduleCourseAnnouncementCommand(CourseId, announcementId, InstructorId, scheduledAt);
            var announcement = CourseAnnouncement.Create(CourseId, InstructorId, "Title", "Content", AnnouncementImportance.High).Value;

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));

            _announcementRepositoryMock
                .Setup(x => x.GetByIdAsync(announcementId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(announcement);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            announcement.ScheduledAt.Should().Be(scheduledAt);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
