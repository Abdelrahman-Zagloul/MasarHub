using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Exams.Commands.ToggleExamPublished;
using MasarHub.Domain.Modules.Exams;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Exams.Commands.ToggleExamPublished
{
    [Trait("UnitTests.Feature.Exams", "ToggleExamPublished")]
    public sealed class ToggleExamPublishedCommandHandlerTests
    {
        private readonly Mock<IExamQuery> _examQueryMock;
        private readonly Mock<IRepository<Exam>> _examRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ToggleExamPublishedCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public ToggleExamPublishedCommandHandlerTests()
        {
            _examQueryMock = new Mock<IExamQuery>();
            _examRepositoryMock = new Mock<IRepository<Exam>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new ToggleExamPublishedCommandHandler(_examQueryMock.Object, _examRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ExamNotFound_ReturnsNotFoundError()
        {
            var command = new ToggleExamPublishedCommand(Guid.NewGuid(), InstructorId, true);

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
            var command = new ToggleExamPublishedCommand(Guid.NewGuid(), InstructorId, true);

            _examQueryMock
                .Setup(x => x.GetExamStateAsync(command.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamState(true, false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Publish_MissingQuestions_ReturnsConflictError()
        {
            var exam = CreateExam();
            var command = new ToggleExamPublishedCommand(exam.Id, InstructorId, true);

            _examQueryMock
                .Setup(x => x.GetExamStateAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamState(true, true, false));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.missing_questions");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Publish_Successful_SetsPublishedToTrue()
        {
            var exam = CreateExam();
            var question = CreateQuestion(exam.Id);
            exam.AddQuestion(question);
            var command = new ToggleExamPublishedCommand(exam.Id, InstructorId, true);

            _examQueryMock
                .Setup(x => x.GetExamStateAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamState(true, true, false));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            exam.IsPublished.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Unpublish_Successful_SetsPublishedToFalse()
        {
            var exam = CreateExam();
            var question = CreateQuestion(exam.Id);
            exam.AddQuestion(question);
            exam.Publish();
            var command = new ToggleExamPublishedCommand(exam.Id, InstructorId, false);

            _examQueryMock
                .Setup(x => x.GetExamStateAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamState(true, true, false));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            exam.IsPublished.Should().BeFalse();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Unpublish_WithAttempts_ReturnsConflictError()
        {
            var exam = CreateExam();
            var question = CreateQuestion(exam.Id);
            exam.AddQuestion(question);
            exam.Publish();
            var command = new ToggleExamPublishedCommand(exam.Id, InstructorId, false);

            _examQueryMock
                .Setup(x => x.GetExamStateAsync(exam.Id, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamState(true, true, true));
            _examRepositoryMock
                .Setup(x => x.GetByIdAsync(exam.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.cannot_unpublish_after_attempts");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
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