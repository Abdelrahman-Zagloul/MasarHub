using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Questions.Queries.GetAllQuestionsByExamId;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Questions.Queries.GetAllQuestionsByExamId
{
    [Trait("UnitTests.Feature.Questions", "GetAllQuestionsByExamId")]
    public sealed class GetAllQuestionsByExamIdQueryHandlerTests
    {
        private readonly Mock<IQuestionQuery> _questionQueryMock;
        private readonly GetAllQuestionsByExamIdQueryHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public GetAllQuestionsByExamIdQueryHandlerTests()
        {
            _questionQueryMock = new Mock<IQuestionQuery>();
            _sut = new GetAllQuestionsByExamIdQueryHandler(_questionQueryMock.Object);
        }

        [Fact]
        public async Task Handle_NoQuestions_ReturnsEmptyList()
        {
            var query = new GetAllQuestionsByExamIdQuery(Guid.NewGuid(), InstructorId, null);

            _questionQueryMock
                .Setup(x => x.GetAllQuestionsByExamIdAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_QuestionsExist_ReturnsMappedResponse()
        {
            var examId = Guid.NewGuid();
            var query = new GetAllQuestionsByExamIdQuery(examId, InstructorId, null);
            var questionId = Guid.NewGuid();
            var optionId = Guid.NewGuid();

            var queryResult = new QuestionQueryResult(questionId, examId, "Sample question?", 10, "TrueFalse")
            {
                Options =
                [
                    new OptionQueryResult(optionId, questionId, "True", true),
                    new OptionQueryResult(Guid.NewGuid(), questionId, "False", false)
                ]
            };

            _questionQueryMock
                .Setup(x => x.GetAllQuestionsByExamIdAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync([queryResult]);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value[0].QuestionId.Should().Be(questionId);
            result.Value[0].ExamId.Should().Be(examId);
            result.Value[0].QuestionText.Should().Be("Sample question?");
            result.Value[0].QuestionMark.Should().Be(10);
            result.Value[0].QuestionType.Should().Be("TrueFalse");
            result.Value[0].Options.Should().HaveCount(2);
            result.Value[0].Options[0].OptionId.Should().Be(optionId);
            result.Value[0].Options[0].Text.Should().Be("True");
            result.Value[0].Options[0].IsCorrect.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_FilterByQuestionType_ReturnsFilteredQuestions()
        {
            var examId = Guid.NewGuid();
            var query = new GetAllQuestionsByExamIdQuery(examId, InstructorId, Domain.Modules.Exams.QuestionType.TrueFalse);
            var questionId = Guid.NewGuid();

            var queryResult = new QuestionQueryResult(questionId, examId, "TrueFalse question?", 10, "TrueFalse")
            {
                Options =
                [
                    new OptionQueryResult(Guid.NewGuid(), questionId, "True", true),
                    new OptionQueryResult(Guid.NewGuid(), questionId, "False", false)
                ]
            };

            _questionQueryMock
                .Setup(x => x.GetAllQuestionsByExamIdAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync([queryResult]);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().ContainSingle();
            result.Value[0].QuestionType.Should().Be("TrueFalse");
        }
    }
}