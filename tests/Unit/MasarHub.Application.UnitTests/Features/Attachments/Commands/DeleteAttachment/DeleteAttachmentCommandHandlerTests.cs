using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Attachments.Commands.DeleteAttachment;
using MasarHub.Domain.Modules.Courses.Lessons;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Attachments.Commands.DeleteAttachment
{
    [Trait("UnitTests.Feature.Attachments", "DeleteAttachment")]
    public sealed class DeleteAttachmentCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<LessonAttachment>> _lessonAttachmentRepoMock;
        private readonly Mock<ILessonQuery> _lessonQueryMock;
        private readonly DeleteAttachmentCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public DeleteAttachmentCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _lessonAttachmentRepoMock = new Mock<IRepository<LessonAttachment>>();
            _lessonQueryMock = new Mock<ILessonQuery>();
            _sut = new DeleteAttachmentCommandHandler(_unitOfWorkMock.Object, _lessonAttachmentRepoMock.Object, _lessonQueryMock.Object);
        }

        [Fact]
        public async Task Handle_AttachmentNotFound_ReturnsNotFoundError()
        {
            var command = new DeleteAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId);

            _lessonAttachmentRepoMock
                .Setup(x => x.GetByIdAsync(command.AttachmentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((LessonAttachment?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "attachment.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AttachmentNotBelongToLesson_ReturnsNotFoundError()
        {
            var command = new DeleteAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId);
            var attachment = CreateAttachment(Guid.NewGuid());

            _lessonAttachmentRepoMock
                .Setup(x => x.GetByIdAsync(command.AttachmentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(attachment);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "attachment.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var lessonId = Guid.NewGuid();
            var command = new DeleteAttachmentCommand(lessonId, Guid.NewGuid(), InstructorId);
            var attachment = CreateAttachment(lessonId);

            _lessonAttachmentRepoMock
                .Setup(x => x.GetByIdAsync(command.AttachmentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(attachment);
            _lessonQueryMock
                .Setup(x => x.IsLessonOwnedByInstructorAsync(lessonId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccess()
        {
            var lessonId = Guid.NewGuid();
            var command = new DeleteAttachmentCommand(lessonId, Guid.NewGuid(), InstructorId);
            var attachment = CreateAttachment(lessonId);

            _lessonAttachmentRepoMock
                .Setup(x => x.GetByIdAsync(command.AttachmentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(attachment);
            _lessonQueryMock
                .Setup(x => x.IsLessonOwnedByInstructorAsync(lessonId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static LessonAttachment CreateAttachment(Guid lessonId)
        {
            return LessonAttachment.Create(lessonId, "public_id", "file.pdf", "application/pdf", 1024).Value;
        }
    }
}
