using FluentAssertions;
using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Attachments.Commands.AddAttachment;
using MasarHub.Domain.Modules.Courses.Lessons;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Attachments.Commands.AddAttachment
{
    [Trait("UnitTests.Feature.Attachments", "AddAttachment")]
    public sealed class AddAttachmentCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly Mock<ILessonQuery> _lessonQueryMock;
        private readonly Mock<IRepository<LessonAttachment>> _lessonAttachmentRepositoryMock;
        private readonly AddAttachmentCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public AddAttachmentCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _fileStorageServiceMock = new Mock<IFileStorageService>();
            _lessonQueryMock = new Mock<ILessonQuery>();
            _lessonAttachmentRepositoryMock = new Mock<IRepository<LessonAttachment>>();
            _sut = new AddAttachmentCommandHandler(_unitOfWorkMock.Object, _fileStorageServiceMock.Object, _lessonQueryMock.Object, _lessonAttachmentRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_LessonNotFound_ReturnsNotFoundError()
        {
            var command = new AddAttachmentCommand(Guid.NewGuid(), InstructorId, CreateFileResource());

            _lessonQueryMock
                .Setup(x => x.GetLessonAttachmentCreationAsync(command.LessonId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LessonAttachmentCreationData(false, false, 0));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var command = new AddAttachmentCommand(Guid.NewGuid(), InstructorId, CreateFileResource());

            _lessonQueryMock
                .Setup(x => x.GetLessonAttachmentCreationAsync(command.LessonId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LessonAttachmentCreationData(true, false, 0));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DomainValidationFails_ReturnsConflictError()
        {
            var command = new AddAttachmentCommand(Guid.NewGuid(), InstructorId, CreateFileResource());

            _lessonQueryMock
                .Setup(x => x.GetLessonAttachmentCreationAsync(command.LessonId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LessonAttachmentCreationData(true, true, 11));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.max_attachment_reached");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UploadFails_ReturnsError()
        {
            var command = new AddAttachmentCommand(Guid.NewGuid(), InstructorId, CreateFileResource());

            _lessonQueryMock
                .Setup(x => x.GetLessonAttachmentCreationAsync(command.LessonId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LessonAttachmentCreationData(true, true, 0));
            _fileStorageServiceMock
                .Setup(x => x.UploadAsync(command.File, FileType.Attachment, StorageFolders.Courses.Attachments, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.NotFound("file.upload_failed"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccess()
        {
            var lessonId = Guid.NewGuid();
            var command = new AddAttachmentCommand(lessonId, InstructorId, CreateFileResource());

            _lessonQueryMock
                .Setup(x => x.GetLessonAttachmentCreationAsync(lessonId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LessonAttachmentCreationData(true, true, 0));
            _fileStorageServiceMock
                .Setup(x => x.UploadAsync(command.File, FileType.Attachment, StorageFolders.Courses.Attachments, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<StoredFile>.Success(new StoredFile("file_key", "file.pdf", "application/pdf", 1024, "https://example.com/file.pdf")));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.AttachmentId.Should().NotBeEmpty();
            result.Value.LessonId.Should().Be(lessonId);
            _lessonAttachmentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<LessonAttachment>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static FileResource CreateFileResource()
        {
            return new FileResource("file.pdf", "application/pdf", new MemoryStream(), 1024);
        }
    }
}
