using FluentAssertions;
using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Lessons.Commands.AddVideoLesson;
using MasarHub.Domain.Modules.Courses.Lessons;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.AddVideoLesson
{
    [Trait("UnitTests.Feature.Lessons", "AddVideoLesson")]
    public sealed class AddVideoLessonCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Lesson>> _lessonRepositoryMock;
        private readonly Mock<ILessonQuery> _lessonQueryMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly AddVideoLessonCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public AddVideoLessonCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _lessonRepositoryMock = new Mock<IRepository<Lesson>>();
            _lessonQueryMock = new Mock<ILessonQuery>();
            _fileStorageServiceMock = new Mock<IFileStorageService>();
            _sut = new AddVideoLessonCommandHandler(_unitOfWorkMock.Object, _lessonRepositoryMock.Object, _lessonQueryMock.Object, _fileStorageServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ModuleNotFound_ReturnsNotFoundError()
        {
            var command = new AddVideoLessonCommand(Guid.NewGuid(), InstructorId, false, "Introduction to Programming", null, "video_public_key_123");

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
            var command = new AddVideoLessonCommand(Guid.NewGuid(), InstructorId, false, "Introduction to Programming", null, "video_public_key_123");

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
        public async Task Handle_MetadataNotFound_ReturnsError()
        {
            var command = new AddVideoLessonCommand(Guid.NewGuid(), InstructorId, false, "Introduction to Programming", null, "video_public_key_123");

            _lessonQueryMock
                .Setup(x => x.GetCreationDataAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LessonCreationData(true, true, 1));
            _fileStorageServiceMock
                .Setup(x => x.GetVideoMetadataAsync(command.FileKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.NotFound("video.metadata_not_found"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _lessonRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Lesson>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DomainFailure_ReturnsFailure()
        {
            var command = new AddVideoLessonCommand(Guid.NewGuid(), InstructorId, false, "", null, "video_public_key_123");

            _lessonQueryMock
                .Setup(x => x.GetCreationDataAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LessonCreationData(true, true, 1));
            _fileStorageServiceMock
                .Setup(x => x.GetVideoMetadataAsync(command.FileKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<StoredFile>.Success(new StoredFile("key", "test.mp4", "video/mp4", 1024, "https://example.com/video.mp4", 120)));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _lessonRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Lesson>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessResponse()
        {
            var moduleId = Guid.NewGuid();
            var command = new AddVideoLessonCommand(moduleId, InstructorId, true, "Introduction to Programming", "Learn the basics", "video_public_key_123");

            _lessonQueryMock
                .Setup(x => x.GetCreationDataAsync(moduleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LessonCreationData(true, true, 1));
            _fileStorageServiceMock
                .Setup(x => x.GetVideoMetadataAsync(command.FileKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<StoredFile>.Success(new StoredFile("key", "test.mp4", "video/mp4", 1024, "https://example.com/video.mp4", 120)));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.ModuleId.Should().Be(moduleId);
            result.Value.Title.Should().Be("Introduction to Programming");
            result.Value.DisplayOrder.Should().Be(1);
            result.Value.IsPreviewable.Should().BeTrue();
            result.Value.Description.Should().Be("Learn the basics");
            result.Value.VideoUrl.Should().Be("https://example.com/video.mp4");
            result.Value.FileName.Should().Be("test.mp4");
            result.Value.FileSizeInByte.Should().Be(1024);
            result.Value.DurationInSeconds.Should().Be(120);
            _lessonRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Lesson>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
