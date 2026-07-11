using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Courses.Commands.CreateCourseAnnouncement;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.CreateCourseAnnouncement
{
    [Trait("UnitTests.Feature.CourseAnnouncement", "CreateCourseAnnouncement")]
    public sealed class CreateCourseAnnouncementCommandHandlerTests
    {
        private readonly Mock<ICourseQuery> _courseQueryMock;
        private readonly Mock<IRepository<CourseAnnouncement>> _announcementRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateCourseAnnouncementCommandHandler _sut;

        public CreateCourseAnnouncementCommandHandlerTests()
        {
            _courseQueryMock = new Mock<ICourseQuery>();
            _announcementRepositoryMock = new Mock<IRepository<CourseAnnouncement>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new CreateCourseAnnouncementCommandHandler(
                _courseQueryMock.Object,
                _announcementRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFoundError()
        {
            var command = CreateValidCommand();

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(command.CourseId, command.InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_found");
        }

        [Fact]
        public async Task Handle_InstructorNotOwner_ReturnsForbiddenError()
        {
            var command = CreateValidCommand();

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(command.CourseId, command.InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesAnnouncementAndReturnsResponse()
        {
            var command = CreateValidCommand();

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(command.CourseId, command.InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.CourseId.Should().Be(command.CourseId);
            result.Value.Title.Should().Be(command.Title);
            result.Value.Content.Should().Be(command.Content);
            result.Value.Importance.Should().Be(command.Importance);
            result.Value.CreatedAt.Should().NotBe(default);

            _announcementRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CourseAnnouncement>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static CreateCourseAnnouncementCommand CreateValidCommand()
            => new(Guid.NewGuid(), Guid.NewGuid(), "Announcement Title", "Announcement Content", AnnouncementImportance.Normal);
    }
}
