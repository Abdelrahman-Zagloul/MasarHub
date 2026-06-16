using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Lessons.Commands.ToggleLessonPreview;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Courses.Lessons;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.ToggleLessonPreview
{
    [Trait("UnitTests.Feature.Lessons", "ToggleLessonPreview")]
    public sealed class ToggleLessonPreviewCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILessonQuery> _lessonQueryMock;
        private readonly Mock<IRepository<Lesson>> _lessonRepositoryMock;
        private readonly ToggleLessonPreviewCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public ToggleLessonPreviewCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _lessonQueryMock = new Mock<ILessonQuery>();
            _lessonRepositoryMock = new Mock<IRepository<Lesson>>();
            _sut = new ToggleLessonPreviewCommandHandler(_unitOfWorkMock.Object, _lessonQueryMock.Object, _lessonRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ModuleNotFound_ReturnsNotFoundError()
        {
            var command = new ToggleLessonPreviewCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, true);

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
            var command = new ToggleLessonPreviewCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, true);

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
            var command = new ToggleLessonPreviewCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, true);

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
        public async Task Handle_EnablePreviewFails_ReturnsConflictError()
        {
            var lesson = CreatePreviewableLesson();
            var command = new ToggleLessonPreviewCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, true);

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Draft));
            _lessonRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(lesson);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.preview_already_enabled");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DisablePreviewFails_ReturnsConflictError()
        {
            var lesson = CreateNonPreviewableLesson();
            var command = new ToggleLessonPreviewCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, false);

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Draft));
            _lessonRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(lesson);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.preview_already_disabled");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EnablePreview_ReturnsSuccess()
        {
            var lesson = CreateNonPreviewableLesson();
            var command = new ToggleLessonPreviewCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, true);

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
        public async Task Handle_DisablePreview_ReturnsSuccess()
        {
            var lesson = CreatePreviewableLesson();
            var command = new ToggleLessonPreviewCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, false);

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

        private static Lesson CreateNonPreviewableLesson()
        {
            return new FakeLesson(false);
        }

        private static Lesson CreatePreviewableLesson()
        {
            return new FakeLesson(true);
        }

        private sealed class FakeLesson : Lesson
        {
            public FakeLesson(bool isPreviewable) : base(Guid.NewGuid(), isPreviewable, "Test", 1, null) { }
        }
    }
}
