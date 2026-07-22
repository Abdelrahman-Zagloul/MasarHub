using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Reviews.Queries.GetCourseReviewById;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Reviews.Queries.GetCourseReviewById
{
    [Trait("UnitTests.Feature.Reviews", "GetCourseReviewById")]
    public sealed class GetCourseReviewByIdQueryHandlerTests
    {
        private readonly Mock<ICourseReviewQuery> _courseReviewQueryMock;
        private readonly GetCourseReviewByIdQueryHandler _sut;

        public GetCourseReviewByIdQueryHandlerTests()
        {
            _courseReviewQueryMock = new Mock<ICourseReviewQuery>();
            _sut = new GetCourseReviewByIdQueryHandler(_courseReviewQueryMock.Object);
        }

        [Fact]
        public async Task Handle_ReviewExists_ReturnsReview()
        {
            var courseId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var query = new GetCourseReviewByIdQuery(courseId, reviewId);
            var response = new CourseReviewResponse(reviewId, courseId, Guid.NewGuid(), "Test User", 4, "Great!", DateTimeOffset.UtcNow, null);

            _courseReviewQueryMock
                .Setup(x => x.GetByIdAsync(courseId, reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(response);
        }

        [Fact]
        public async Task Handle_ReviewNotFound_ReturnsNotFoundError()
        {
            var query = new GetCourseReviewByIdQuery(Guid.NewGuid(), Guid.NewGuid());

            _courseReviewQueryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseReviewResponse?)null);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course_review.not_found");
        }
    }
}
