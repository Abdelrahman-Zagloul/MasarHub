using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Questions.Queries.GetQuestionById;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Questions.Queries.GetQuestionById
{
    [Trait("UnitTests.Feature.Questions", "GetQuestionById")]
    public sealed class GetQuestionByIdQueryHandlerTests
    {
        private readonly Mock<IQuestionQuery> _questionQueryMock;
        private readonly GetQuestionByIdQueryHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public GetQuestionByIdQueryHandlerTests()
        {
            _questionQueryMock = new Mock<IQuestionQuery>();
            _sut = new GetQuestionByIdQueryHandler(_questionQueryMock.Object);
        }

        [Fact]
        public async Task Handle_QuestionNotFound_ReturnsNotFoundError()
        {
            var query = new GetQuestionByIdQuery(Guid.NewGuid(), Guid.NewGuid(), InstructorId);

            _questionQueryMock
                .Setup(x => x.GetQuestionByIdAsync(query.QuestionId, query.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((QuestionQueryResult?)null);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "question.not_found");
        }

        [Fact]
        public async Task Handle_QuestionFound_ReturnsMappedResponse()
        {
            var query = new GetQuestionByIdQuery(Guid.NewGuid(), Guid.NewGuid(), InstructorId);
            var queryResult = new QuestionQueryResult(query.QuestionId, query.ExamId, "Test question?", 10, "TrueFalse")
            {
                Options =
                [
                    new OptionQueryResult(Guid.NewGuid(),query.QuestionId, "True", true),
                    new OptionQueryResult(Guid.NewGuid(), query.QuestionId,"False", false)
                ]
            };

            _questionQueryMock
                .Setup(x => x.GetQuestionByIdAsync(query.QuestionId, query.ExamId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(new QuestionDetailsResponse(
                query.QuestionId,
                query.ExamId,
                "Test question?",
                10,
                "TrueFalse",
                queryResult.Options.Select(o => new OptionResponse(o.Id, query.QuestionId, o.Text, o.IsCorrect)).ToList()));
        }
    }
}
