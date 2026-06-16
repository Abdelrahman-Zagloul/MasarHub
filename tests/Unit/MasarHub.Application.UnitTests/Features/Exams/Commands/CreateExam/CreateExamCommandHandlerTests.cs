using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Exams.Commands.CreateExam;
using MasarHub.Domain.Modules.Exams;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Exams.Commands.CreateExam
{
    [Trait("UnitTests.Feature.Exams", "CreateExam")]
    public sealed class CreateExamCommandHandlerTests
    {
        private readonly Mock<IExamQuery> _examQueryMock;
        private readonly Mock<IRepository<Exam>> _examRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateExamCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public CreateExamCommandHandlerTests()
        {
            _examQueryMock = new Mock<IExamQuery>();
            _examRepositoryMock = new Mock<IRepository<Exam>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new CreateExamCommandHandler(_examQueryMock.Object, _examRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFoundError()
        {
            var command = new CreateExamCommand(Guid.NewGuid(), InstructorId, "Final Exam", 70, 2, null, null, null);

            _examQueryMock
                .Setup(x => x.GetCreationDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamCreationData(false, false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_found");
            _examRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Exam>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var command = new CreateExamCommand(Guid.NewGuid(), InstructorId, "Final Exam", 70, 2, null, null, null);

            _examQueryMock
                .Setup(x => x.GetCreationDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamCreationData(true, false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _examRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Exam>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ModuleNotFound_ReturnsNotFoundError()
        {
            var moduleId = Guid.NewGuid();
            var command = new CreateExamCommand(Guid.NewGuid(), InstructorId, "Final Exam", 70, 2, moduleId, null, null);

            _examQueryMock
                .Setup(x => x.GetCreationDataAsync(command.CourseId, moduleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamCreationData(true, true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.not_found");
            _examRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Exam>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DomainFailure_ReturnsFailure()
        {
            var command = new CreateExamCommand(Guid.NewGuid(), InstructorId, "Final Exam", -1, 1, null, null, null);

            _examQueryMock
                .Setup(x => x.GetCreationDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamCreationData(true, true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _examRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Exam>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidExam_ReturnsSuccessResponse()
        {
            var courseId = Guid.NewGuid();
            var command = new CreateExamCommand(courseId, InstructorId, "Final Exam 2026", 70, 2, null, "Comprehensive final exam", 120);

            _examQueryMock
                .Setup(x => x.GetCreationDataAsync(courseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamCreationData(true, true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be("Final Exam 2026");
            result.Value.PassingScorePercentage.Should().Be(70);
            result.Value.MaxAttempts.Should().Be(2);
            result.Value.CourseId.Should().Be(courseId);
            _examRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Exam>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidExamWithModule_ReturnsSuccessResponse()
        {
            var courseId = Guid.NewGuid();
            var moduleId = Guid.NewGuid();
            var command = new CreateExamCommand(courseId, InstructorId, "Module Quiz", 60, 3, moduleId, "Test your knowledge", 30);

            _examQueryMock
                .Setup(x => x.GetCreationDataAsync(courseId, moduleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamCreationData(true, true, true));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be("Module Quiz");
            _examRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Exam>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
