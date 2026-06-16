using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Lessons.Commands.AddArticleLesson;
using MasarHub.Domain.Modules.Courses.Lessons;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.AddArticleLesson
{
    [Trait("UnitTests.Feature.Lessons", "AddArticleLesson")]
    public sealed class AddArticleLessonCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Lesson>> _lessonRepositoryMock;
        private readonly Mock<ILessonQuery> _lessonQueryMock;
        private readonly AddArticleLessonCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public AddArticleLessonCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _lessonRepositoryMock = new Mock<IRepository<Lesson>>();
            _lessonQueryMock = new Mock<ILessonQuery>();
            _sut = new AddArticleLessonCommandHandler(_unitOfWorkMock.Object, _lessonRepositoryMock.Object, _lessonQueryMock.Object);
        }

        [Fact]
        public async Task Handle_ModuleNotFound_ReturnsNotFoundError()
        {
            var command = new AddArticleLessonCommand(Guid.NewGuid(), InstructorId, false, "Introduction to Programming", "Content here", null);

            _lessonQueryMock
                .Setup(x => x.GetCreationDataAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LessonCreationData(false, false, 1));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.not_found");
            _lessonRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Lesson>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var command = new AddArticleLessonCommand(Guid.NewGuid(), InstructorId, false, "Introduction to Programming", "Content here", null);

            _lessonQueryMock
                .Setup(x => x.GetCreationDataAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LessonCreationData(true, false, 1));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _lessonRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Lesson>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DomainFailure_ReturnsFailure()
        {
            var command = new AddArticleLessonCommand(Guid.NewGuid(), InstructorId, false, "", "Content here", null);

            _lessonQueryMock
                .Setup(x => x.GetCreationDataAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LessonCreationData(true, true, 1));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _lessonRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Lesson>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessResponse()
        {
            var moduleId = Guid.NewGuid();
            var command = new AddArticleLessonCommand(moduleId, InstructorId, false, "Introduction to Programming", "Content here", "Description text");

            _lessonQueryMock
                .Setup(x => x.GetCreationDataAsync(moduleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LessonCreationData(true, true, 1));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.ModuleId.Should().Be(moduleId);
            result.Value.Title.Should().Be("Introduction to Programming");
            result.Value.Content.Should().Be("Content here");
            result.Value.DisplayOrder.Should().Be(1);
            result.Value.IsPreviewable.Should().BeFalse();
            result.Value.Description.Should().Be("Description text");
            _lessonRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Lesson>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
