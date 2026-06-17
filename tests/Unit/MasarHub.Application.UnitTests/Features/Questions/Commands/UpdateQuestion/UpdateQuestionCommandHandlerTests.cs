using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Questions.Commands.UpdateQuestion;
using MasarHub.Domain.Modules.Exams;
using Moq;
using System.Linq.Expressions;

namespace MasarHub.Application.UnitTests.Features.Questions.Commands.UpdateQuestion
{
    [Trait("UnitTests.Feature.Questions", "UpdateQuestion")]
    public sealed class UpdateQuestionCommandHandlerTests
    {
        private readonly Mock<IExamQuery> _examQueryMock;
        private readonly Mock<IRepository<Question>> _questionRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateQuestionCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public UpdateQuestionCommandHandlerTests()
        {
            _examQueryMock = new Mock<IExamQuery>();
            _questionRepositoryMock = new Mock<IRepository<Question>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new UpdateQuestionCommandHandler(_examQueryMock.Object, _questionRepositoryMock.Object, _unitOfWorkMock.Object);
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
        public async Task Handle_ExamIsPublished_ReturnsBadRequestError()
        {
            var command = CreateValidCommand();

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, true));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.cannot_modify_published_exam");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuestionNotFound_ReturnsNotFoundError()
        {
            var command = CreateValidCommand();

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _questionRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Question, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Question, object>>[]>()))
                .ReturnsAsync((Question?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "question.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UpdateText_UpdatesText()
        {
            var question = CreateTrueFalseQuestion(Guid.NewGuid());
            var command = new UpdateQuestionCommand(question.ExamId, question.Id, InstructorId, "Updated text", null, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(question.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _questionRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Question, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Question, object>>[]>()))
                .ReturnsAsync(question);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            question.QuestionText.Should().Be("Updated text");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateMark_UpdatesMark()
        {
            var question = CreateTrueFalseQuestion(Guid.NewGuid());
            var command = new UpdateQuestionCommand(question.ExamId, question.Id, InstructorId, null, 15, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(question.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _questionRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Question, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Question, object>>[]>()))
                .ReturnsAsync(question);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            question.QuestionMark.Should().Be(15);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateOptions_UpdatesInPlace()
        {
            var question = CreateTrueFalseQuestion(Guid.NewGuid());
            var existingOptions = question.Options.ToList();
            var updatedOptions = new List<Question.OptionUpdateInput>
            {
                new(existingOptions[0].Id, "Absolutely true", true),
                new(existingOptions[1].Id, "Absolutely false", false)
            };
            var command = new UpdateQuestionCommand(question.ExamId, question.Id, InstructorId, null, null, updatedOptions);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(question.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _questionRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Question, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Question, object>>[]>()))
                .ReturnsAsync(question);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            question.Options.Should().HaveCount(2);
            question.Options.Should().Contain(o => o.Id == existingOptions[0].Id && o.Text == "Absolutely true" && o.IsCorrect);
            question.Options.Should().Contain(o => o.Id == existingOptions[1].Id && o.Text == "Absolutely false" && !o.IsCorrect);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateAllFields_UpdatesAll()
        {
            var question = CreateTrueFalseQuestion(Guid.NewGuid());
            var existingOptions = question.Options.ToList();
            var updatedOptions = new List<Question.OptionUpdateInput>
            {
                new(existingOptions[0].Id, "Agree", true),
                new(existingOptions[1].Id, "Disagree", false)
            };
            var command = new UpdateQuestionCommand(question.ExamId, question.Id, InstructorId, "New question text", 20, updatedOptions);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(question.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _questionRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Question, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Question, object>>[]>()))
                .ReturnsAsync(question);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            question.QuestionText.Should().Be("New question text");
            question.QuestionMark.Should().Be(20);
            question.Options.Should().HaveCount(2);
            question.Options.Should().Contain(o => o.Id == existingOptions[0].Id && o.Text == "Agree" && o.IsCorrect);
            question.Options.Should().Contain(o => o.Id == existingOptions[1].Id && o.Text == "Disagree" && !o.IsCorrect);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NoFieldsProvided_DoesNothing()
        {
            var question = CreateTrueFalseQuestion(Guid.NewGuid());
            var originalText = question.QuestionText;
            var originalMark = question.QuestionMark;
            var originalOptions = question.Options.ToList();
            var command = new UpdateQuestionCommand(question.ExamId, question.Id, InstructorId, null, null, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(question.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _questionRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Question, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Question, object>>[]>()))
                .ReturnsAsync(question);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            question.QuestionText.Should().Be(originalText);
            question.QuestionMark.Should().Be(originalMark);
            question.Options.Should().HaveCount(originalOptions.Count);
            question.Options.Select(o => o.Text).Should().BeEquivalentTo(originalOptions.Select(o => o.Text));
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidText_ReturnsDomainError()
        {
            var question = CreateTrueFalseQuestion(Guid.NewGuid());
            var command = new UpdateQuestionCommand(question.ExamId, question.Id, InstructorId, "", null, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(question.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _questionRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Question, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Question, object>>[]>()))
                .ReturnsAsync(question);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidMark_ReturnsDomainError()
        {
            var question = CreateTrueFalseQuestion(Guid.NewGuid());
            var command = new UpdateQuestionCommand(question.ExamId, question.Id, InstructorId, null, 0, null);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(question.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _questionRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Question, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Question, object>>[]>()))
                .ReturnsAsync(question);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_OptionNotFound_ReturnsDomainError()
        {
            var question = CreateTrueFalseQuestion(Guid.NewGuid());
            var updatedOptions = new List<Question.OptionUpdateInput>
            {
                new(Guid.NewGuid(), "Orphan option", true)
            };
            var command = new UpdateQuestionCommand(question.ExamId, question.Id, InstructorId, null, null, updatedOptions);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(question.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _questionRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Question, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Question, object>>[]>()))
                .ReturnsAsync(question);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.option_not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidOptionsAfterUpdate_ReturnsDomainError()
        {
            var question = CreateTrueFalseQuestion(Guid.NewGuid());
            var existingOptions = question.Options.ToList();
            var updatedOptions = new List<Question.OptionUpdateInput>
            {
                new(existingOptions[0].Id, "Still true", false),
                new(existingOptions[1].Id, "Still false", false)
            };
            var command = new UpdateQuestionCommand(question.ExamId, question.Id, InstructorId, null, null, updatedOptions);

            _examQueryMock
                .Setup(x => x.GetUpdateDataAsync(question.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamUpdateData(true, true, false));
            _questionRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<Question, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Question, object>>[]>()))
                .ReturnsAsync(question);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        private static UpdateQuestionCommand CreateValidCommand()
        {
            return new UpdateQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, "Updated text", 10, null);
        }

        private static Question CreateTrueFalseQuestion(Guid examId)
        {
            return Question.Create(
                examId,
                "Is the sky blue?",
                5,
                QuestionType.TrueFalse,
                new List<Question.OptionInput>
                {
                    new("True", true),
                    new("False", false)
                }).Value;
        }
    }
}
