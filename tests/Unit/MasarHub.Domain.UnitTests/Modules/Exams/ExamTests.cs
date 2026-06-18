using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Exams;

namespace MasarHub.Domain.UnitTests.Modules.Exams
{
    [Trait("UnitTests.Domain.Exams", "Exam")]
    public sealed class ExamTests
    {
        private static readonly Guid ValidCourseId = Guid.NewGuid();
        private const string ValidTitle = "Final Exam";
        private const int ValidPassingScore = 60;
        private const int ValidMaxAttempts = 3;

        #region Create

        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            var result = Exam.Create(ValidCourseId, ValidTitle, ValidPassingScore, ValidMaxAttempts);

            Assert.True(result.IsSuccess);
            Assert.Equal(ValidCourseId, result.Value.CourseId);
            Assert.Equal(ValidTitle, result.Value.Title);
            Assert.Equal(ValidPassingScore, result.Value.PassingScorePercentage);
            Assert.Equal(ValidMaxAttempts, result.Value.MaxAttempts);
            Assert.Null(result.Value.ModuleId);
            Assert.Null(result.Value.Description);
            Assert.Null(result.Value.DurationInMinutes);
            Assert.False(result.Value.IsPublished);
            Assert.Empty(result.Value.Questions);
        }

        [Fact]
        public void Create_WithOptionalFields_ReturnsSuccess()
        {
            var moduleId = Guid.NewGuid();
            var description = "Comprehensive final exam";
            var duration = 120;

            var result = Exam.Create(ValidCourseId, ValidTitle, ValidPassingScore, ValidMaxAttempts, moduleId, description, duration);

            Assert.True(result.IsSuccess);
            Assert.Equal(moduleId, result.Value.ModuleId);
            Assert.Equal(description, result.Value.Description);
            Assert.Equal(duration, result.Value.DurationInMinutes);
        }

        [Fact]
        public void Create_EmptyCourseId_ReturnsError()
        {
            var result = Exam.Create(Guid.Empty, ValidTitle, ValidPassingScore, ValidMaxAttempts);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("courseId").Code, result.Error.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidTitle_ReturnsError(string? title)
        {
            var result = Exam.Create(ValidCourseId, title!, ValidPassingScore, ValidMaxAttempts);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.NullOrEmpty("title").Code, result.Error.Code);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_InvalidMaxAttempts_ReturnsError(int maxAttempts)
        {
            var result = Exam.Create(ValidCourseId, ValidTitle, ValidPassingScore, maxAttempts);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.NegativeOrZero("maxAttempts").Code, result.Error.Code);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void Create_InvalidPassingScore_ReturnsError(int passingScore)
        {
            var result = Exam.Create(ValidCourseId, ValidTitle, passingScore, ValidMaxAttempts);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.InvalidPassingScore.Code, result.Error.Code);
        }

        [Fact]
        public void Create_EmptyModuleId_ReturnsError()
        {
            var result = Exam.Create(ValidCourseId, ValidTitle, ValidPassingScore, ValidMaxAttempts, Guid.Empty);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("moduleId").Code, result.Error.Code);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_InvalidDuration_ReturnsError(int duration)
        {
            var result = Exam.Create(ValidCourseId, ValidTitle, ValidPassingScore, ValidMaxAttempts, null, null, duration);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.NegativeOrZero("durationMinutes").Code, result.Error.Code);
        }

        #endregion

        #region UpdateTitle

        [Fact]
        public void UpdateTitle_ValidInput_ReturnsSuccess()
        {
            var exam = CreateValidExam();
            var newTitle = "Updated Exam Title";

            var result = exam.UpdateTitle(newTitle);

            Assert.True(result.IsSuccess);
            Assert.Equal(newTitle, exam.Title);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateTitle_InvalidInput_ReturnsError(string? title)
        {
            var exam = CreateValidExam();

            var result = exam.UpdateTitle(title!);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void UpdateTitle_PublishedExam_ReturnsError()
        {
            var exam = CreatePublishedExam();

            var result = exam.UpdateTitle("New Title");

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.CannotModifyPublishedExam.Code, result.Error.Code);
        }

        #endregion

        #region UpdateDescription

        [Fact]
        public void UpdateDescription_ValidInput_ReturnsSuccess()
        {
            var exam = CreateValidExam();
            var newDescription = "Updated description";

            var result = exam.UpdateDescription(newDescription);

            Assert.True(result.IsSuccess);
            Assert.Equal(newDescription, exam.Description);
        }

        [Fact]
        public void UpdateDescription_Null_ClearsDescription()
        {
            var exam = Exam.Create(ValidCourseId, ValidTitle, ValidPassingScore, ValidMaxAttempts, null, "Original description").Value;

            var result = exam.UpdateDescription(null);

            Assert.True(result.IsSuccess);
            Assert.Null(exam.Description);
        }

        [Fact]
        public void UpdateDescription_PublishedExam_ReturnsError()
        {
            var exam = CreatePublishedExam();

            var result = exam.UpdateDescription("New description");

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.CannotModifyPublishedExam.Code, result.Error.Code);
        }

        #endregion

        #region UpdateMaxAttempts

        [Fact]
        public void UpdateMaxAttempts_ValidInput_ReturnsSuccess()
        {
            var exam = CreateValidExam();
            var newMax = 5;

            var result = exam.UpdateMaxAttempts(newMax);

            Assert.True(result.IsSuccess);
            Assert.Equal(newMax, exam.MaxAttempts);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void UpdateMaxAttempts_InvalidInput_ReturnsError(int maxAttempts)
        {
            var exam = CreateValidExam();

            var result = exam.UpdateMaxAttempts(maxAttempts);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region UpdatePassingScore

        [Fact]
        public void UpdatePassingScore_ValidInput_ReturnsSuccess()
        {
            var exam = CreateValidExam();
            var newScore = 70;

            var result = exam.UpdatePassingScore(newScore);

            Assert.True(result.IsSuccess);
            Assert.Equal(newScore, exam.PassingScorePercentage);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void UpdatePassingScore_InvalidInput_ReturnsError(int score)
        {
            var exam = CreateValidExam();

            var result = exam.UpdatePassingScore(score);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.InvalidPassingScore.Code, result.Error.Code);
        }

        #endregion

        #region UpdateDuration

        [Fact]
        public void UpdateDuration_ValidInput_ReturnsSuccess()
        {
            var exam = CreateValidExam();
            var newDuration = 90;

            var result = exam.UpdateDuration(newDuration);

            Assert.True(result.IsSuccess);
            Assert.Equal(newDuration, exam.DurationInMinutes);
        }

        [Fact]
        public void UpdateDuration_Null_ClearsDuration()
        {
            var exam = Exam.Create(ValidCourseId, ValidTitle, ValidPassingScore, ValidMaxAttempts, null, null, 60).Value;

            var result = exam.UpdateDuration(null);

            Assert.True(result.IsSuccess);
            Assert.Null(exam.DurationInMinutes);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void UpdateDuration_InvalidInput_ReturnsError(int duration)
        {
            var exam = CreateValidExam();

            var result = exam.UpdateDuration(duration);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region Publish

        [Fact]
        public void Publish_HasQuestions_ReturnsSuccess()
        {
            var exam = CreateExamWithQuestions();

            var result = exam.Publish();

            Assert.True(result.IsSuccess);
            Assert.True(exam.IsPublished);
        }

        [Fact]
        public void Publish_AlreadyPublished_ReturnsError()
        {
            var exam = CreatePublishedExam();

            var result = exam.Publish();

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.AlreadyPublished.Code, result.Error.Code);
        }

        [Fact]
        public void Publish_NoQuestions_ReturnsError()
        {
            var exam = CreateValidExam();

            var result = exam.Publish();

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.MissingQuestions.Code, result.Error.Code);
        }

        #endregion

        #region Unpublish

        [Fact]
        public void Unpublish_Valid_ReturnsSuccess()
        {
            var exam = CreatePublishedExam();

            var result = exam.Unpublish(false);

            Assert.True(result.IsSuccess);
            Assert.False(exam.IsPublished);
        }

        [Fact]
        public void Unpublish_AlreadyUnpublished_ReturnsError()
        {
            var exam = CreateValidExam();

            var result = exam.Unpublish(false);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.AlreadyUnpublished.Code, result.Error.Code);
        }

        [Fact]
        public void Unpublish_HasAttempts_ReturnsError()
        {
            var exam = CreatePublishedExam();

            var result = exam.Unpublish(true);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.CannotUnpublishAfterAttempts.Code, result.Error.Code);
        }

        #endregion

        #region AddQuestion

        [Fact]
        public void AddQuestion_Valid_ReturnsSuccess()
        {
            var exam = CreateValidExam();
            var question = CreateValidQuestion(exam.Id);

            var result = exam.AddQuestion(question);

            Assert.True(result.IsSuccess);
            Assert.Single(exam.Questions);
        }

        [Fact]
        public void AddQuestion_NullQuestion_ReturnsError()
        {
            var exam = CreateValidExam();

            var result = exam.AddQuestion(null!);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void AddQuestion_WrongExamId_ReturnsError()
        {
            var exam = CreateValidExam();
            var question = CreateValidQuestion(Guid.NewGuid());

            var result = exam.AddQuestion(question);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.InvalidQuestionExamRelation.Code, result.Error.Code);
        }

        [Fact]
        public void AddQuestion_PublishedExam_ReturnsError()
        {
            var exam = CreatePublishedExam();
            var question = CreateValidQuestion(exam.Id);

            var result = exam.AddQuestion(question);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.CannotModifyPublishedExam.Code, result.Error.Code);
        }

        #endregion

        #region RemoveQuestion

        [Fact]
        public void RemoveQuestion_Valid_ReturnsSuccess()
        {
            var exam = CreateExamWithQuestions();
            var questionId = exam.Questions.First().Id;

            var result = exam.RemoveQuestion(questionId);

            Assert.True(result.IsSuccess);
            Assert.Single(exam.Questions);
        }

        [Fact]
        public void RemoveQuestion_NotFound_ReturnsError()
        {
            var exam = CreateValidExam();

            var result = exam.RemoveQuestion(Guid.NewGuid());

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.QuestionNotFound.Code, result.Error.Code);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_NoAttempts_ReturnsSuccess()
        {
            var exam = CreateValidExam();

            var result = exam.Delete(false);

            Assert.True(result.IsSuccess);
            Assert.True(exam.IsDeleted);
            Assert.NotNull(exam.DeletedAt);
        }

        [Fact]
        public void Delete_HasAttempts_ReturnsError()
        {
            var exam = CreateValidExam();

            var result = exam.Delete(true);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.CannotDeleteExamWithSubmissions.Code, result.Error.Code);
        }

        [Fact]
        public void Delete_PublishedExam_ReturnsError()
        {
            var exam = CreatePublishedExam();

            var result = exam.Delete(false);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.CannotModifyPublishedExam.Code, result.Error.Code);
        }

        #endregion

        #region TotalMarks

        [Fact]
        public void TotalMarks_ReturnsSumOfQuestionMarks()
        {
            var exam = CreateExamWithQuestions();

            var total = exam.TotalMarks();

            Assert.Equal(20m, total);
        }

        [Fact]
        public void TotalMarks_NoQuestions_ReturnsZero()
        {
            var exam = CreateValidExam();

            var total = exam.TotalMarks();

            Assert.Equal(0m, total);
        }

        #endregion

        #region GetPassingScore

        [Fact]
        public void GetPassingScore_ReturnsCorrectPercentage()
        {
            var exam = CreateExamWithQuestions();

            var passingScore = exam.GetPassingScore();

            Assert.Equal(12m, passingScore);
        }

        #endregion

        #region CanAttempt

        [Fact]
        public void CanAttempt_BelowMax_ReturnsTrue()
        {
            var exam = CreateValidExam();

            var canAttempt = exam.CanAttempt(1);

            Assert.True(canAttempt);
        }

        [Fact]
        public void CanAttempt_AtMax_ReturnsFalse()
        {
            var exam = CreateValidExam();

            var canAttempt = exam.CanAttempt(3);

            Assert.False(canAttempt);
        }

        [Fact]
        public void CanAttempt_ExceedsMax_ReturnsFalse()
        {
            var exam = CreateValidExam();

            var canAttempt = exam.CanAttempt(5);

            Assert.False(canAttempt);
        }

        #endregion


        private static Exam CreateValidExam()
        {
            return Exam.Create(ValidCourseId, ValidTitle, ValidPassingScore, ValidMaxAttempts).Value;
        }

        private static Question CreateValidQuestion(Guid examId)
        {
            var options = new[]
            {
                new Question.OptionInput("Paris", true),
                new Question.OptionInput("London", false),
                new Question.OptionInput("Berlin", false),
                new Question.OptionInput("Madrid", false),
            };

            return Question.Create(examId, "What is the capital of France?", 10m, QuestionType.SingleChoice, options).Value;
        }

        private static Exam CreateExamWithQuestions()
        {
            var exam = CreateValidExam();
            var optionInputs1 = new[]
            {
                new Question.OptionInput("Paris", true),
                new Question.OptionInput("London", false),
                new Question.OptionInput("Berlin", false),
                new Question.OptionInput("Madrid", false),
            };
            var q1 = Question.Create(exam.Id, "Capital of France?", 10m, QuestionType.SingleChoice, optionInputs1).Value;
            exam.AddQuestion(q1);

            var optionInputs2 = new[]
            {
                new Question.OptionInput("True", true),
                new Question.OptionInput("False", false),
            };
            var q2 = Question.Create(exam.Id, "Earth is round?", 10m, QuestionType.TrueFalse, optionInputs2).Value;
            exam.AddQuestion(q2);

            return exam;
        }

        private static Exam CreatePublishedExam()
        {
            var exam = CreateExamWithQuestions();
            exam.Publish();
            return exam;
        }
    }
}