using FluentAssertions;
using MasarHub.Application.Features.Questions.Queries.GetQuestionById;

namespace MasarHub.Application.UnitTests.Features.Questions.Queries.GetQuestionById
{
    [Trait("UnitTests.Feature.Questions", "GetQuestionById")]
    public sealed class GetQuestionByIdQueryValidatorTests
    {
        private readonly GetQuestionByIdQueryValidator _sut = new();
        private static readonly Guid InstructorId = Guid.NewGuid();

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = new GetQuestionByIdQuery(Guid.NewGuid(), Guid.NewGuid(), InstructorId);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyExamId_ReturnsValidationError()
        {
            var query = new GetQuestionByIdQuery(Guid.Empty, Guid.NewGuid(), InstructorId);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ExamId");
        }

        [Fact]
        public void Validate_EmptyQuestionId_ReturnsValidationError()
        {
            var query = new GetQuestionByIdQuery(Guid.NewGuid(), Guid.Empty, InstructorId);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "QuestionId");
        }
    }
}
