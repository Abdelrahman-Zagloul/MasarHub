using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Questions.Commands.CreateQuestion;
using MasarHub.Domain.Modules.Exams;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Questions.Commands.CreateQuestion
{
    [Trait("UnitTests.Feature.Questions", "CreateQuestion")]
    public sealed class CreateQuestionCommandHandlerTests
    {
        private readonly Mock<IExamQuery> _examQueryMock;
        private readonly Mock<IRepository<Exam>> _examRepositoryMock;
        private readonly Mock<IRepository<Question>> _questionRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateQuestionCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public CreateQuestionCommandHandlerTests()
        {
            _examQueryMock = new Mock<IExamQuery>();
            _examRepositoryMock = new Mock<IRepository<Exam>>();
            _questionRepositoryMock = new Mock<IRepository<Question>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new CreateQuestionCommandHandler(_examQueryMock.Object, _examRepositoryMock.Object, _unitOfWorkMock.Object, _questionRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ExamNotFound_ReturnsNotFoundError()
        {
            var command = CreateValidCommand();

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
            var command = CreateValidCommand();

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
            var command = CreateValidCommand();

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
        public async Task Handle_InvalidQuestionMark_ReturnsDomainError()
        {
            var exam = CreateDraftExam();
            var command = new CreateQuestionCommand(exam.Id, InstructorId, "Sample question?", 0, QuestionType.SingleChoice,
            [
                new Question.OptionInput("Wrong A", false),
                new Question.OptionInput("Correct", true),
                new Question.OptionInput("Wrong B", false)
            ]);

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
        public async Task Handle_ValidSingleChoiceQuestion_ReturnsSuccessResponse()
        {
            var exam = CreateDraftExam();
            var command = new CreateQuestionCommand(exam.Id, InstructorId, "What is the capital of France?", 10, QuestionType.SingleChoice,
            [
                new Question.OptionInput("Paris", true),
                new Question.OptionInput("London", false),
                new Question.OptionInput("Berlin", false)
            ]);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.QuestionText.Should().Be("What is the capital of France?");
            result.Value.QuestionMark.Should().Be(10);
            result.Value.QuestionType.Should().Be(QuestionType.SingleChoice);
            result.Value.Options.Should().HaveCount(3);
            result.Value.Options.Should().Contain(o => o.Text == "Paris" && o.IsCorrect);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidTrueFalseQuestion_ReturnsSuccessResponse()
        {
            var exam = CreateDraftExam();
            var command = new CreateQuestionCommand(exam.Id, InstructorId, "The sky is blue.", 5, QuestionType.TrueFalse,
            [
                new Question.OptionInput("True", true),
                new Question.OptionInput("False", false)
            ]);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.QuestionType.Should().Be(QuestionType.TrueFalse);
            result.Value.Options.Should().HaveCount(2);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidMultipleChoiceQuestion_ReturnsSuccessResponse()
        {
            var exam = CreateDraftExam();
            var command = new CreateQuestionCommand(exam.Id, InstructorId, "Select programming languages.", 15, QuestionType.MultipleChoice,
            [
                new Question.OptionInput("C#", true),
                new Question.OptionInput("Java", true),
                new Question.OptionInput("HTML", false),
                new Question.OptionInput("Python", true)
            ]);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.QuestionType.Should().Be(QuestionType.MultipleChoice);
            result.Value.Options.Should().HaveCount(4);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static CreateQuestionCommand CreateValidCommand()
        {
            return new CreateQuestionCommand(Guid.NewGuid(), InstructorId, "Sample question?", 10, QuestionType.SingleChoice,
            [
                new Question.OptionInput("Wrong A", false),
                new Question.OptionInput("Correct", true),
                new Question.OptionInput("Wrong B", false)
            ]);
        }

        private static Exam CreateDraftExam()
        {
            return Exam.Create(Guid.NewGuid(), "Test Exam", 70, 3).Value;
        }
    }
}
