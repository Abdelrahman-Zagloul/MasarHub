using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Courses.Commands.SubmitCourseForApproval;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.SubmitCourseForApproval
{
    [Trait("UnitTests.Feature.Courses", "SubmitCourseForApproval")]
    public sealed class SubmitCourseForApprovalCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Course>> _courseRepositoryMock;
        private readonly Mock<ICourseQuery> _courseQueryMock;
        private readonly SubmitCourseForApprovalCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public SubmitCourseForApprovalCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _courseRepositoryMock = new Mock<IRepository<Course>>();
            _courseQueryMock = new Mock<ICourseQuery>();
            _sut = new SubmitCourseForApprovalCommandHandler(_unitOfWorkMock.Object, _courseRepositoryMock.Object, _courseQueryMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFoundError()
        {
            var command = new SubmitCourseForApprovalCommand(Guid.NewGuid(), InstructorId);

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(command.CourseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Course?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotInstructor_ReturnsForbiddenError()
        {
            var course = Course.Create("Title", "slug", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid(), Guid.NewGuid()).Value;
            var command = new SubmitCourseForApprovalCommand(course.Id, InstructorId);

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NoLectures_ReturnsBadRequestError()
        {
            var course = Course.Create("Title", "slug", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, InstructorId, Guid.NewGuid()).Value;
            var command = new SubmitCourseForApprovalCommand(course.Id, InstructorId);

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);
            _courseQueryMock
                .Setup(x => x.HasLecturesAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.cannot_submit_empty_course");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidCourse_SubmitsForApproval()
        {
            var course = Course.Create("Title", "slug", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, InstructorId, Guid.NewGuid()).Value;
            var command = new SubmitCourseForApprovalCommand(course.Id, InstructorId);

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);
            _courseQueryMock
                .Setup(x => x.HasLecturesAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            course.Status.Should().Be(CourseStatus.PendingApproval);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AlreadySubmitted_ReturnsDomainError()
        {
            var course = Course.Create("Title", "slug", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, InstructorId, Guid.NewGuid()).Value;
            course.SubmitForApproval();
            var command = new SubmitCourseForApprovalCommand(course.Id, InstructorId);

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);
            _courseQueryMock
                .Setup(x => x.HasLecturesAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
