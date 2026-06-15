using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Courses.Commands.ApproveCourse;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.ApproveCourse
{
    [Trait("UnitTests.Feature.Courses", "ApproveCourse")]
    public sealed class ApproveCourseCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Course>> _courseRepositoryMock;
        private readonly ApproveCourseCommandHandler _sut;
        private static readonly Guid AdminId = Guid.NewGuid();

        public ApproveCourseCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _courseRepositoryMock = new Mock<IRepository<Course>>();
            _sut = new ApproveCourseCommandHandler(_unitOfWorkMock.Object, _courseRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFoundError()
        {
            var command = new ApproveCourseCommand(Guid.NewGuid(), AdminId);

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(command.CourseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Course?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidCourse_ApprovesPublication()
        {
            var course = Course.Create("Title", "slug", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid(), Guid.NewGuid()).Value;
            course.SubmitForApproval();
            var command = new ApproveCourseCommand(course.Id, AdminId);

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            course.Status.Should().Be(CourseStatus.Published);
            course.ApprovedBy.Should().Be(AdminId);
            course.PublishedAt.Should().NotBeNull();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AlreadyPublished_ReturnsDomainError()
        {
            var course = Course.Create("Title", "slug", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid(), Guid.NewGuid()).Value;
            course.SubmitForApproval();
            course.ApprovePublication(AdminId);
            var command = new ApproveCourseCommand(course.Id, AdminId);

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotPendingApproval_ReturnsDomainError()
        {
            var course = Course.Create("Title", "slug", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid(), Guid.NewGuid()).Value;
            var command = new ApproveCourseCommand(course.Id, AdminId);

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
