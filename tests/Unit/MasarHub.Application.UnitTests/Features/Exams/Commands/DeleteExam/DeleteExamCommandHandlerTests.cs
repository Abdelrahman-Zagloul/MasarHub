using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Exams.Commands.DeleteExam;
using MasarHub.Domain.Modules.Exams;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Exams.Commands.DeleteExam
{
    [Trait("UnitTests.Feature.Exams", "DeleteExam")]
    public sealed class DeleteExamCommandHandlerTests
    {
        private readonly Mock<IExamQuery> _examQueryMock;
        private readonly Mock<IRepository<Exam>> _examRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteExamCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public DeleteExamCommandHandlerTests()
        {
            _examQueryMock = new Mock<IExamQuery>();
            _examRepositoryMock = new Mock<IRepository<Exam>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new DeleteExamCommandHandler(_examQueryMock.Object, _examRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ExamNotFound_ReturnsNotFoundError()
        {
            var command = new DeleteExamCommand(Guid.NewGuid(), InstructorId);

            _examQueryMock
                .Setup(x => x.GetExamStateAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamState(false, false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var command = new DeleteExamCommand(Guid.NewGuid(), InstructorId);

            _examQueryMock
                .Setup(x => x.GetExamStateAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamState(true, false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_HasAttempts_ReturnsBadRequestError()
        {
            var exam = CreateExam();
            exam.AddQuestion(CreateQuestion(exam.Id));

            var command = new DeleteExamCommand(exam.Id, InstructorId);

            _examQueryMock
                .Setup(x => x.GetExamStateAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamState(true, true, true));

            _examRepositoryMock
               .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
               .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.cannot_delete_has_submission");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Published_ReturnsConflictError()
        {
            var exam = CreateExam();
            exam.AddQuestion(CreateQuestion(exam.Id));
            exam.Publish();

            var command = new DeleteExamCommand(exam.Id, InstructorId);

            _examQueryMock
                .Setup(x => x.GetExamStateAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamState(true, true, false));

            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.cannot_modify_published_exam");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_SuccessfulDelete_MarksExamAsDeleted()
        {
            var exam = CreateExam();
            var command = new DeleteExamCommand(exam.Id, InstructorId);

            _examQueryMock
                .Setup(x => x.GetExamStateAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamState(true, true, false));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            exam.IsDeleted.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static Exam CreateExam()
            => Exam.Create(Guid.NewGuid(), "Original Title", 70, 2, null, "Original description", 60).Value;
        private static Question CreateQuestion(Guid examId)
        {
            var option1 = new Question.OptionInput("Option 1", true);
            var option2 = new Question.OptionInput("Option 2", false);
            var options = new List<Question.OptionInput> { option1, option2 };
            var questionResult = Question.Create(examId, "Question 1", 1m, QuestionType.SingleChoice, options);
            return questionResult.Value;
        }
    }
}
