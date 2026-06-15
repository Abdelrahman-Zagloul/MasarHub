using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Courses.Commands.CreateCourse;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.CreateCourse
{
    [Trait("UnitTests.Feature.Courses", "CreateCourse")]
    public sealed class CreateCourseCommandHandlerTests
    {
        private readonly Mock<IRepository<Course>> _courseRepositoryMock;
        private readonly Mock<ICourseQuery> _courseQueryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly CreateCourseCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public CreateCourseCommandHandlerTests()
        {
            _courseRepositoryMock = new Mock<IRepository<Course>>();
            _courseQueryMock = new Mock<ICourseQuery>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(InstructorId);
            _sut = new CreateCourseCommandHandler(_courseRepositoryMock.Object, _courseQueryMock.Object, _unitOfWorkMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCourse_ReturnsSuccessResponse()
        {
            var categoryId = Guid.NewGuid();
            var command = new CreateCourseCommand("Programming 101", "Learn programming", 49.99m, CourseLanguage.English, CourseLevel.Beginner, categoryId);

            _courseQueryMock
                .Setup(x => x.GetCreationDataAsync(It.IsAny<string>(), categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseCreationData(true, 0));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be("Programming 101");
            result.Value.Status.Should().Be(CourseStatus.Draft);
            result.Value.InstructorId.Should().Be(InstructorId);
            result.Value.CategoryId.Should().Be(categoryId);
            _courseRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CategoryNotFound_ReturnsNotFoundError()
        {
            var command = new CreateCourseCommand("Programming 101", "Learn programming", 49.99m, CourseLanguage.English, CourseLevel.Beginner, Guid.NewGuid());

            _courseQueryMock
                .Setup(x => x.GetCreationDataAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseCreationData(false, 0));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "category.not_found");
            _courseRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_SlugExists_AppendsNumber()
        {
            var categoryId = Guid.NewGuid();
            var command = new CreateCourseCommand("Programming 101", "Learn programming", 49.99m, CourseLanguage.English, CourseLevel.Beginner, categoryId);

            _courseQueryMock
                .Setup(x => x.GetCreationDataAsync(It.IsAny<string>(), categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseCreationData(true, 2));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _courseRepositoryMock.Verify(x => x.AddAsync(It.Is<Course>(c => c.Slug.EndsWith("-3")), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
