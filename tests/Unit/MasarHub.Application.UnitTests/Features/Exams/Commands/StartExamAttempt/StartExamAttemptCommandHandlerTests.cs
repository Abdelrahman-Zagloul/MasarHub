using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Exams.Commands.StartExamAttempt;
using MasarHub.Domain.Modules.Exams;
using Moq;
using System.Reflection;

namespace MasarHub.Application.UnitTests.Features.Exams.Commands.StartExamAttempt
{
    [Trait("UnitTests.Feature.Exams", "StartExamAttempt")]
    public sealed class StartExamAttemptCommandHandlerTests
    {
        private readonly Mock<IExamQuery> _examQueryMock;
        private readonly Mock<IRepository<ExamAttempt>> _examAttemptRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly StartExamAttemptCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly Guid CourseId = Guid.NewGuid();
        private static readonly Guid ExamId = Guid.NewGuid();

        public StartExamAttemptCommandHandlerTests()
        {
            _examQueryMock = new Mock<IExamQuery>();
            _examAttemptRepositoryMock = new Mock<IRepository<ExamAttempt>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new StartExamAttemptCommandHandler(
                _examQueryMock.Object,
                _examAttemptRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_NotEnrolled_ReturnsForbidden()
        {
            var command = CreateCommand();

            _examQueryMock
                .Setup(x => x.GetAttemptStartDataAsync(ExamId, CourseId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamAttemptStartData(false, 0, null));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_enrolled");
            _examAttemptRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ExamAttempt>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExamNotFound_ReturnsNotFound()
        {
            var command = CreateCommand();

            _examQueryMock
                .Setup(x => x.GetAttemptStartDataAsync(ExamId, CourseId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamAttemptStartData(true, 0, null));
            _examQueryMock
                .Setup(x => x.GetExamDetailsAsync(ExamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Exam?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.not_found");
            _examAttemptRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ExamAttempt>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExamNotPublished_ReturnsNotFound()
        {
            var command = CreateCommand();
            var exam = CreateExam(isPublished: false);

            _examQueryMock
                .Setup(x => x.GetAttemptStartDataAsync(ExamId, CourseId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamAttemptStartData(true, 0, null));
            _examQueryMock
                .Setup(x => x.GetExamDetailsAsync(ExamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam.not_found");
            _examAttemptRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ExamAttempt>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_MaxAttemptsReached_ReturnsBadRequest()
        {
            var command = CreateCommand();
            var exam = CreateExam(isPublished: true, maxAttempts: 2);

            _examQueryMock
                .Setup(x => x.GetAttemptStartDataAsync(ExamId, CourseId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamAttemptStartData(true, 2, null));
            _examQueryMock
                .Setup(x => x.GetExamDetailsAsync(ExamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "exam_attempt.max_attempts_reached");
            _examAttemptRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ExamAttempt>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExistingInProgressAttempt_ReturnsExistingAttempt()
        {
            var command = CreateCommand();
            var attemptId = Guid.NewGuid();
            var exam = CreateExamWithQuestions(isPublished: true, maxAttempts: 3);
            var existingAttempt = CreateAttempt(attemptId, ExamId, UserId);

            _examQueryMock
                .Setup(x => x.GetAttemptStartDataAsync(ExamId, CourseId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamAttemptStartData(true, 0, attemptId));
            _examQueryMock
                .Setup(x => x.GetExamDetailsAsync(ExamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);
            _examAttemptRepositoryMock
                .Setup(x => x.GetByIdAsync(attemptId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAttempt);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.AttemptId.Should().Be(attemptId);
            result.Value.ExamTitle.Should().Be("Test Exam");
            _examAttemptRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ExamAttempt>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Success_CreatesNewAttempt()
        {
            var command = CreateCommand();
            var exam = CreateExamWithQuestions(isPublished: true, maxAttempts: 3);

            _examQueryMock
                .Setup(x => x.GetAttemptStartDataAsync(ExamId, CourseId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExamAttemptStartData(true, 0, null));
            _examQueryMock
                .Setup(x => x.GetExamDetailsAsync(ExamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exam);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.ExamTitle.Should().Be("Test Exam");
            _examAttemptRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ExamAttempt>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static StartExamAttemptCommand CreateCommand()
        {
            return new StartExamAttemptCommand(CourseId, ExamId, UserId);
        }

        private static Exam CreateExam(bool isPublished, int maxAttempts = 3)
        {
            return CreateExamWithQuestions(isPublished, maxAttempts, addQuestions: false);
        }

        private static Exam CreateExamWithQuestions(bool isPublished, int maxAttempts = 3, bool addQuestions = true)
        {
            var exam = (Exam)Activator.CreateInstance(typeof(Exam), nonPublic: true)!;

            SetProperty(exam, nameof(Exam.Id), ExamId);
            SetProperty(exam, nameof(Exam.CourseId), CourseId);
            SetProperty(exam, nameof(Exam.Title), "Test Exam");
            SetProperty(exam, nameof(Exam.PassingScorePercentage), 50);
            SetProperty(exam, nameof(Exam.MaxAttempts), maxAttempts);
            SetProperty(exam, nameof(Exam.DurationInMinutes), 30);
            SetProperty(exam, nameof(Exam.IsPublished), isPublished);

            if (addQuestions)
            {
                var questions = CreateQuestions(exam);
                SetField(exam, "_questions", questions);
            }

            return exam;
        }

        private static List<Question> CreateQuestions(Exam exam)
        {
            var question = (Question)Activator.CreateInstance(typeof(Question), nonPublic: true)!;

            SetProperty(question, nameof(Question.Id), Guid.NewGuid());
            SetProperty(question, nameof(Question.ExamId), exam.Id);
            SetProperty(question, nameof(Question.QuestionText), "Sample question?");
            SetProperty(question, nameof(Question.QuestionMark), 10m);
            SetProperty(question, nameof(Question.QuestionType), QuestionType.SingleChoice);

            var option = (Option)Activator.CreateInstance(typeof(Option), nonPublic: true)!;
            SetProperty(option, nameof(Option.Id), Guid.NewGuid());
            SetProperty(option, nameof(Option.QuestionId), question.Id);
            SetProperty(option, nameof(Option.Text), "Option A");
            SetProperty(option, nameof(Option.IsCorrect), true);

            var option2 = (Option)Activator.CreateInstance(typeof(Option), nonPublic: true)!;
            SetProperty(option2, nameof(Option.Id), Guid.NewGuid());
            SetProperty(option2, nameof(Option.QuestionId), question.Id);
            SetProperty(option2, nameof(Option.Text), "Option B");
            SetProperty(option2, nameof(Option.IsCorrect), false);

            SetField(question, "_options", new List<Option> { option, option2 });

            return [question];
        }

        private static ExamAttempt CreateAttempt(Guid id, Guid examId, Guid userId)
        {
            var attempt = (ExamAttempt)Activator.CreateInstance(typeof(ExamAttempt), nonPublic: true)!;

            SetProperty(attempt, "Id", id);
            SetProperty(attempt, "ExamId", examId);
            SetProperty(attempt, "UserId", userId);
            SetProperty(attempt, "Status", ExamAttemptStatus.InProgress);
            SetProperty(attempt, "StartedAt", DateTimeOffset.UtcNow);

            return attempt;
        }

        private static void SetProperty(object target, string propertyName, object value)
        {
            var prop = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            prop!.SetValue(target, value);
        }

        private static void SetField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field!.SetValue(target, value);
        }
    }
}
