using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Lessons.Commands.UpdateLesson;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Courses.Lessons;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.UpdateLesson
{
    [Trait("UnitTests.Feature.Lessons", "UpdateLesson")]
    public sealed class UpdateLessonCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILessonQuery> _lessonQueryMock;
        private readonly Mock<IRepository<Lesson>> _lessonRepositoryMock;
        private readonly UpdateLessonCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public UpdateLessonCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _lessonQueryMock = new Mock<ILessonQuery>();
            _lessonRepositoryMock = new Mock<IRepository<Lesson>>();
            _sut = new UpdateLessonCommandHandler(_unitOfWorkMock.Object, _lessonQueryMock.Object, _lessonRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ModuleNotFound_ReturnsNotFoundError()
        {
            var command = new UpdateLessonCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, "Updated Title", null);

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(false, false, CourseStatus.Draft));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var command = new UpdateLessonCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, "Updated Title", null);

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, false, CourseStatus.Draft));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_LessonNotFound_ReturnsNotFoundError()
        {
            var command = new UpdateLessonCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, "Updated Title", null);

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Draft));
            _lessonRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Lesson?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UpdateTitle_ReturnsSuccess()
        {
            var lesson = new FakeLesson("Original Title", null);
            var command = new UpdateLessonCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, "Updated Title", null);

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Draft));
            _lessonRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(lesson);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateDescription_ReturnsSuccess()
        {
            var lesson = new FakeLesson("Original Title", null);
            var command = new UpdateLessonCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, null, "Updated Description");

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Draft));
            _lessonRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(lesson);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NoChangesProvided_ReturnsSuccess()
        {
            var lesson = new FakeLesson("Original Title", "Original Description");
            var command = new UpdateLessonCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, null, null);

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Draft));
            _lessonRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(lesson);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private sealed class FakeLesson : Lesson
        {
            public FakeLesson(string title, string? description) : base(Guid.NewGuid(), false, title, 1, description) { }
        }
    }
}
