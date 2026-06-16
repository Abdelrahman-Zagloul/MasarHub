using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Exams.Commands.UpdateExam;
using MasarHub.Domain.Modules.Exams;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Exams.Commands.UpdateExam
{
    [Trait("UnitTests.Feature.Exams", "UpdateExam")]
    public sealed class UpdateExamCommandHandlerTests
    {
        private readonly Mock<IExamQuery> _examQueryMock;
        private readonly Mock<IRepository<Exam>> _examRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateExamCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public UpdateExamCommandHandlerTests()
        {
            _examQueryMock = new Mock<IExamQuery>();
            _examRepositoryMock = new Mock<IRepository<Exam>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new UpdateExamCommandHandler(_examQueryMock.Object, _examRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ExamNotFound_ReturnsNotFoundError()
        {
            var command = new UpdateExamCommand(Guid.NewGuid(), InstructorId, "New Title", null, null, null, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var command = new UpdateExamCommand(Guid.NewGuid(), InstructorId, "New Title", null, null, null, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExamNotFoundAfterCheck_ReturnsNotFoundError()
        {
            var command = new UpdateExamCommand(Guid.NewGuid(), InstructorId, "New Title", null, null, null, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(command.ExamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Exam?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UpdateTitle_UpdatesTitle()
        {
            var exam = CreateExam();
            var command = new UpdateExamCommand(exam.Id, InstructorId, "New Title", null, null, null, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            exam.Title.Should().Be("New Title");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateDescription_UpdatesDescription()
        {
            var exam = CreateExam();
            var command = new UpdateExamCommand(exam.Id, InstructorId, null, "New description", null, null, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            exam.Description.Should().Be("New description");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateMaxAttempts_UpdatesMaxAttempts()
        {
            var exam = CreateExam();
            var command = new UpdateExamCommand(exam.Id, InstructorId, null, null, 3, null, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            exam.MaxAttempts.Should().Be(3);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdatePassingScore_UpdatesPassingScore()
        {
            var exam = CreateExam();
            var command = new UpdateExamCommand(exam.Id, InstructorId, null, null, null, 80, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            exam.PassingScorePercentage.Should().Be(80);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateDuration_UpdatesDuration()
        {
            var exam = CreateExam();
            var command = new UpdateExamCommand(exam.Id, InstructorId, null, null, null, null, 90);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            exam.DurationInMinutes.Should().Be(90);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DurationNotProvided_DoesNotUpdateDuration()
        {
            var exam = CreateExam();
            var originalDuration = exam.DurationInMinutes;
            var command = new UpdateExamCommand(exam.Id, InstructorId, null, null, null, null, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            exam.DurationInMinutes.Should().Be(originalDuration);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidDuration_ReturnsDomainError()
        {
            var exam = CreateExam();
            var command = new UpdateExamCommand(exam.Id, InstructorId, null, null, null, null, 0);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ClearDescription_ClearsDescription()
        {
            var exam = CreateExam();
            var command = new UpdateExamCommand(exam.Id, InstructorId, null, string.Empty, null, null, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            exam.Description.Should().BeEmpty();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static Exam CreateExam()
            => Exam.Create(Guid.NewGuid(), "Original Title", 70, 2, null, "Original description", 60).Value;
    }
}
