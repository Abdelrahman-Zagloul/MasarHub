using FluentAssertions;
using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Lessons.Commands.UpdateVideoThumbnail;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Courses.Lessons;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.UpdateVideoThumbnail
{
    [Trait("UnitTests.Feature.Lessons", "UpdateVideoThumbnail")]
    public sealed class UpdateVideoThumbnailCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILessonQuery> _lessonQueryMock;
        private readonly Mock<IRepository<Lesson>> _lessonRepositoryMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly UpdateVideoThumbnailCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public UpdateVideoThumbnailCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _lessonQueryMock = new Mock<ILessonQuery>();
            _lessonRepositoryMock = new Mock<IRepository<Lesson>>();
            _fileStorageServiceMock = new Mock<IFileStorageService>();
            _sut = new UpdateVideoThumbnailCommandHandler(_unitOfWorkMock.Object, _lessonQueryMock.Object, _lessonRepositoryMock.Object, _fileStorageServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ModuleNotFound_ReturnsNotFoundError()
        {
            var command = new UpdateVideoThumbnailCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, CreateFileResource());

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
            var command = new UpdateVideoThumbnailCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, CreateFileResource());

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
            var command = new UpdateVideoThumbnailCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, CreateFileResource());

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Draft));
            _lessonRepositoryMock
                .Setup(x => x.GetByIdAsync(command.LessonId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Lesson?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_LessonIsNotVideo_ReturnsBadRequestError()
        {
            var lesson = new FakeArticleLesson();
            var command = new UpdateVideoThumbnailCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, CreateFileResource());

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Draft));
            _lessonRepositoryMock
                .Setup(x => x.GetByIdAsync(command.LessonId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(lesson);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.not_video_lesson");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UploadThumbnailFails_ReturnsError()
        {
            var lesson = CreateVideoLesson();
            var command = new UpdateVideoThumbnailCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, CreateFileResource());

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Draft));
            _lessonRepositoryMock
                .Setup(x => x.GetByIdAsync(command.LessonId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(lesson);
            _fileStorageServiceMock
                .Setup(x => x.UploadAsync(command.File, FileType.Image, StorageFolders.Courses.Thumbnails, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.NotFound("file.upload_failed"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccess()
        {
            var lesson = CreateVideoLesson();
            var command = new UpdateVideoThumbnailCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, CreateFileResource());

            _lessonQueryMock
                .Setup(x => x.GetCourseStateAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseState(true, true, CourseStatus.Draft));
            _lessonRepositoryMock
                .Setup(x => x.GetByIdAsync(command.LessonId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(lesson);
            _fileStorageServiceMock
                .Setup(x => x.UploadAsync(command.File, FileType.Image, StorageFolders.Courses.Thumbnails, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<StoredFile>.Success(new StoredFile("thumb_key", "thumb.jpg", "image/jpeg", 1024, "https://example.com/thumb.jpg")));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static VideoLesson CreateVideoLesson()
        {
            return VideoLesson.Create(Guid.NewGuid(), false, "Test Video", 1, null, "public_id", "test.mp4", 1024, 60).Value;
        }

        private static FileResource CreateFileResource()
        {
            return new FileResource("thumb.jpg", "image/jpeg", new MemoryStream(), 1024);
        }

        private sealed class FakeArticleLesson : Lesson
        {
            public FakeArticleLesson() : base(Guid.NewGuid(), false, "Article", 1, null) { }
        }
    }
}
