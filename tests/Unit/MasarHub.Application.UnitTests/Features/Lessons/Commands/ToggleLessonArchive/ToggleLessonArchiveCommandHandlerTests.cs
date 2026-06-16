using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Lessons.Commands.ToggleLessonArchive;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Courses.Lessons;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.ToggleLessonArchive
{
    [Trait("UnitTests.Feature.Lessons", "ToggleLessonArchive")]
    public sealed class ToggleLessonArchiveCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILessonQuery> _lessonQueryMock;
        private readonly Mock<IRepository<Lesson>> _lessonRepositoryMock;
        private readonly ToggleLessonArchiveCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public ToggleLessonArchiveCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _lessonQueryMock = new Mock<ILessonQuery>();
            _lessonRepositoryMock = new Mock<IRepository<Lesson>>();
            _sut = new ToggleLessonArchiveCommandHandler(_unitOfWorkMock.Object, _lessonQueryMock.Object, _lessonRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ModuleNotFound_ReturnsNotFoundError()
        {
            var command = new ToggleLessonArchiveCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, true);

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
            var command = new ToggleLessonArchiveCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, true);

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
            var command = new ToggleLessonArchiveCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, true);

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
        public async Task Handle_ArchiveFails_ReturnsConflictError()
        {
            var lesson = CreatePublishedLesson();
            var command = new ToggleLessonArchiveCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, true);

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Draft));
            _lessonRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(lesson);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.cannot_archive_unpublished_lesson");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UnarchiveFails_ReturnsConflictError()
        {
            var lesson = CreateActiveLesson();
            var command = new ToggleLessonArchiveCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, false);

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Published));
            _lessonRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(lesson);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.already_not_archived");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ArchiveLesson_ReturnsSuccess()
        {
            var lesson = CreatePublishedLesson();
            var command = new ToggleLessonArchiveCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, true);

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Published));
            _lessonRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(lesson);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UnarchiveLesson_ReturnsSuccess()
        {
            var lesson = CreateArchivedLesson();
            var command = new ToggleLessonArchiveCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, false);

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

        private static Lesson CreateActiveLesson()
        {
            return new FakeLesson(false, false);
        }

        private static Lesson CreatePublishedLesson()
        {
            return new FakeLesson(false, false);
        }

        private static Lesson CreateArchivedLesson()
        {
            return new FakeLesson(false, true);
        }

        private sealed class FakeLesson : Lesson
        {
            public FakeLesson(bool isPreviewable, bool isArchived) : base(Guid.NewGuid(), isPreviewable, "Test", 1, null)
            {
                if (isArchived)
                {
                    var statusField = typeof(Lesson).GetField("<LessonStatus>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    statusField!.SetValue(this, LessonStatus.Archived);
                }
            }
        }
    }
}
