using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Progress.Commands.CompleteLesson;
using MasarHub.Domain.Modules.Courses.Lessons;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Progress.Commands.CompleteLesson
{
    [Trait("UnitTests.Feature.Progress", "CompleteLesson")]
    public sealed class CompleteLessonCommandHandlerTests
    {
        private readonly Mock<IProgressQuery> _progressQueryMock;
        private readonly Mock<IRepository<LessonProgress>> _lessonProgressRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CompleteLessonCommandHandler _sut;

        public CompleteLessonCommandHandlerTests()
        {
            _progressQueryMock = new Mock<IProgressQuery>();
            _lessonProgressRepositoryMock = new Mock<IRepository<LessonProgress>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new CompleteLessonCommandHandler(
                _progressQueryMock.Object,
                _lessonProgressRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_NotEnrolled_ReturnsForbiddenError()
        {
            var command = new CompleteLessonCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _progressQueryMock
                .Setup(x => x.GetCreateProgressDataAsync(command.UserId, command.CourseId, command.LessonId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateProgressData(false, false, false, null));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_enrolled");
            _lessonProgressRepositoryMock.Verify(x => x.AddAsync(It.IsAny<LessonProgress>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_LessonNotFound_ReturnsNotFoundError()
        {
            var command = new CompleteLessonCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _progressQueryMock
                .Setup(x => x.GetCreateProgressDataAsync(command.UserId, command.CourseId, command.LessonId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateProgressData(true, false, false, null));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.not_found");
            _lessonProgressRepositoryMock.Verify(x => x.AddAsync(It.IsAny<LessonProgress>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AlreadyCompleted_ReturnsConflictError()
        {
            var command = new CompleteLessonCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _progressQueryMock
                .Setup(x => x.GetCreateProgressDataAsync(command.UserId, command.CourseId, command.LessonId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateProgressData(true, true, true, Guid.NewGuid()));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "progress.already_completed");
            _lessonProgressRepositoryMock.Verify(x => x.AddAsync(It.IsAny<LessonProgress>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesProgressAndSaves()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var lessonId = Guid.NewGuid();
            var moduleId = Guid.NewGuid();
            var command = new CompleteLessonCommand(userId, courseId, lessonId);

            _progressQueryMock
                .Setup(x => x.GetCreateProgressDataAsync(userId, courseId, lessonId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateProgressData(true, true, false, moduleId));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _lessonProgressRepositoryMock.Verify(
                x => x.AddAsync(It.Is<LessonProgress>(lp => lp.UserId == userId && lp.LessonId == lessonId && lp.ModuleId == moduleId && lp.CourseId == courseId), It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
