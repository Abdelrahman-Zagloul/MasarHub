using FluentAssertions;
using MasarHub.Application.Features.Questions.Queries.GetAllQuestionsByExamId;

namespace MasarHub.Application.UnitTests.Features.Questions.Queries.GetAllQuestionsByExamId
{
    [Trait("UnitTests.Feature.Questions", "GetAllQuestionsByExamId")]
    public sealed class GetAllQuestionsByExamIdQueryValidatorTests
    {
        private readonly GetAllQuestionsByExamIdQueryValidator _sut = new();
        private static readonly Guid InstructorId = Guid.NewGuid();

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = new GetAllQuestionsByExamIdQuery(Guid.NewGuid(), InstructorId, null);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyExamId_ReturnsValidationError()
        {
            var query = new GetAllQuestionsByExamIdQuery(Guid.Empty, InstructorId, null);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ExamId");
        }

        [Fact]
        public void Validate_EmptyInstructorId_ReturnsValidationError()
        {
            var query = new GetAllQuestionsByExamIdQuery(Guid.NewGuid(), Guid.Empty, null);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "InstructorId");
        }

        [Fact]
        public void Validate_ValidQuestionType_ReturnsNoErrors()
        {
            var query = new GetAllQuestionsByExamIdQuery(Guid.NewGuid(), InstructorId, Domain.Modules.Exams.QuestionType.TrueFalse);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_InvalidQuestionType_ReturnsValidationError()
        {
            var query = new GetAllQuestionsByExamIdQuery(Guid.NewGuid(), InstructorId, (Domain.Modules.Exams.QuestionType)99);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "QuestionType");
        }

        [Fact]
        public void Validate_NullQuestionType_ReturnsNoErrors()
        {
            var query = new GetAllQuestionsByExamIdQuery(Guid.NewGuid(), InstructorId, null);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }
    }
}