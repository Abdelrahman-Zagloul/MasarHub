using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Courses.Commands.UpdateCourseRequirements;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.UpdateCourseRequirements
{
    [Trait("UnitTests.Feature.Courses", "UpdateCourseRequirements")]
    public sealed class UpdateCourseRequirementsCommandHandlerTests
    {
        private readonly Mock<IRepository<Course>> _courseRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly UpdateCourseRequirementsCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public UpdateCourseRequirementsCommandHandlerTests()
        {
            _courseRepositoryMock = new Mock<IRepository<Course>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(InstructorId);
            _sut = new UpdateCourseRequirementsCommandHandler(_courseRepositoryMock.Object, _unitOfWorkMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFoundError()
        {
            var command = new UpdateCourseRequirementsCommand(Guid.NewGuid(), ["Basic math"]);

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
            var command = new UpdateCourseRequirementsCommand(course.Id, ["Basic math"]);

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidRequirements_UpdatesRequirements()
        {
            var course = Course.Create("Title", "slug", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, InstructorId, Guid.NewGuid()).Value;
            var command = new UpdateCourseRequirementsCommand(course.Id, ["Basic math", "PC access"]);

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            course.Requirements.Should().HaveCount(2);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
