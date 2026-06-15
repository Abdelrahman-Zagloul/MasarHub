using FluentAssertions;
using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Courses.Commands.UpdateCourseThumbnail;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.UpdateCourseThumbnail
{
    [Trait("UnitTests.Feature.Courses", "UpdateCourseThumbnail")]
    public sealed class UpdateCourseThumbnailCommandHandlerTests
    {
        private readonly Mock<IRepository<Course>> _courseRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly UpdateCourseThumbnailCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public UpdateCourseThumbnailCommandHandlerTests()
        {
            _courseRepositoryMock = new Mock<IRepository<Course>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(InstructorId);
            _fileStorageServiceMock = new Mock<IFileStorageService>();
            _sut = new UpdateCourseThumbnailCommandHandler(_courseRepositoryMock.Object, _unitOfWorkMock.Object, _currentUserServiceMock.Object, _fileStorageServiceMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFoundError()
        {
            var command = new UpdateCourseThumbnailCommand(Guid.NewGuid(), new FileResource("test.jpg", "image/jpeg", new MemoryStream(), 1024));

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
            var command = new UpdateCourseThumbnailCommand(course.Id, new FileResource("test.jpg", "image/jpeg", new MemoryStream(), 1024));

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UploadSucceeds_ReturnsUrlAndDeletesOldThumbnail()
        {
            var course = Course.Create("Title", "slug", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, InstructorId, Guid.NewGuid()).Value;
            course.UpdateThumbnailPublicId("old-public-id");
            var command = new UpdateCourseThumbnailCommand(course.Id, new FileResource("test.jpg", "image/jpeg", new MemoryStream(), 1024));
            var storedFile = new StoredFile("new-public-id", "test.jpg", "image/jpeg", 1024, "https://example.com/thumb.jpg");

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);
            _fileStorageServiceMock
                .Setup(x => x.UploadAsync(It.IsAny<FileResource>(), FileType.Image, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<StoredFile>.Success(storedFile));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("https://example.com/thumb.jpg");
            _fileStorageServiceMock.Verify(x => x.DeleteAsync("old-public-id", FileType.Image, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UploadSucceeds_WithNoOldThumbnail_DoesNotDelete()
        {
            var course = Course.Create("Title", "slug", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, InstructorId, Guid.NewGuid()).Value;
            var command = new UpdateCourseThumbnailCommand(course.Id, new FileResource("test.jpg", "image/jpeg", new MemoryStream(), 1024));
            var storedFile = new StoredFile("new-public-id", "test.jpg", "image/jpeg", 1024, "https://example.com/thumb.jpg");

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);
            _fileStorageServiceMock
                .Setup(x => x.UploadAsync(It.IsAny<FileResource>(), FileType.Image, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<StoredFile>.Success(storedFile));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _fileStorageServiceMock.Verify(x => x.DeleteAsync(It.IsAny<string>(), FileType.Image, It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UploadFails_ReturnsError()
        {
            var course = Course.Create("Title", "slug", "Description", 0, CourseLanguage.English, CourseLevel.Beginner, InstructorId, Guid.NewGuid()).Value;
            var command = new UpdateCourseThumbnailCommand(course.Id, new FileResource("test.jpg", "image/jpeg", new MemoryStream(), 1024));

            _courseRepositoryMock
                .Setup(x => x.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);
            _fileStorageServiceMock
                .Setup(x => x.UploadAsync(It.IsAny<FileResource>(), FileType.Image, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.BadRequest("upload.failed"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
