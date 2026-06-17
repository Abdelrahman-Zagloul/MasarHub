using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Questions.Commands.DeleteQuestion;
using MasarHub.Domain.Modules.Exams;
using Moq;
using System.Linq.Expressions;

namespace MasarHub.Application.UnitTests.Features.Questions.Commands.DeleteQuestion
{
    [Trait("UnitTests.Feature.Questions", "DeleteQuestion")]
    public sealed class DeleteQuestionCommandHandlerTests
    {
        private readonly Mock<IExamQuery> _examQueryMock;
        private readonly Mock<IRepository<Exam>> _examRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteQuestionCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public DeleteQuestionCommandHandlerTests()
        {
            _examQueryMock = new Mock<IExamQuery>();
            _examRepositoryMock = new Mock<IRepository<Exam>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new DeleteQuestionCommandHandler(_examQueryMock.Object, _examRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ExamNotFound_ReturnsNotFoundError()
        {
            var command = CreateValidCommand();

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(false, false, false));

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
                .ReturnsAsync(new ExamUpdateData(true, false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExamNotLoaded_ReturnsNotFoundError()
        {
            var command = CreateValidCommand();

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _examRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Exam, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Exam, object>>[]>()))
                .ReturnsAsync((Exam?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuestionNotFoundInExam_ReturnsDomainError()
        {
            var exam = CreateExamWithQuestions();
            var command = new DeleteQuestionCommand(exam.Id, Guid.NewGuid(), InstructorId);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _examRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Exam, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Exam, object>>[]>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.question_not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidRequest_RemovesQuestionAndSaves()
        {
            var exam = CreateExamWithQuestions();
            var questionToDelete = exam.Questions.First();
            var command = new DeleteQuestionCommand(exam.Id, questionToDelete.Id, InstructorId);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _examRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Exam, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Exam, object>>[]>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            exam.Questions.Should().NotContain(q => q.Id == questionToDelete.Id);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static DeleteQuestionCommand CreateValidCommand()
        {
            return new DeleteQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId);
        }

        private static Exam CreateExamWithQuestions()
        {
            var exam = Exam.Create(
                Guid.NewGuid(),
                "Test Exam",
                70,
                3,
                null,
                "Test description",
                30).Value;

            var question = Question.Create(
                exam.Id,
                "Is the sky blue?",
                5,
                QuestionType.TrueFalse,
                new List<Question.OptionInput>
                {
                    new("True", true),
                    new("False", false)
                }).Value;

            exam.AddQuestion(question);
            return exam;
        }
    }
}
