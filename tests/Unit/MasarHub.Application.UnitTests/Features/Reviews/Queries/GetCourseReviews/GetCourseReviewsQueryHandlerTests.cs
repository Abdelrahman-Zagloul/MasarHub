using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Reviews.Queries.GetCourseReviewById;
using MasarHub.Application.Features.Reviews.Queries.GetCourseReviews;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Reviews.Queries.GetCourseReviews
{
    [Trait("UnitTests.Feature.Reviews", "GetCourseReviews")]
    public sealed class GetCourseReviewsQueryHandlerTests
    {
        private readonly Mock<ICourseReviewQuery> _courseReviewQueryMock;
        private readonly GetCourseReviewsQueryHandler _sut;

        public GetCourseReviewsQueryHandlerTests()
        {
            _courseReviewQueryMock = new Mock<ICourseReviewQuery>();
            _sut = new GetCourseReviewsQueryHandler(_courseReviewQueryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsPaginatedResult()
        {
            var courseId = Guid.NewGuid();
            var query = new GetCourseReviewsQuery(courseId, 1, 10);
            var reviews = new List<CourseReviewResponse>
            {
                new(Guid.NewGuid(), courseId, Guid.NewGuid(), "User A", 5, "Excellent", DateTimeOffset.UtcNow, null),
                new(Guid.NewGuid(), courseId, Guid.NewGuid(), "User B", 4, "Good", DateTimeOffset.UtcNow, null)
            };

            _courseReviewQueryMock
                .Setup(x => x.GetAllAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<CourseReviewResponse>(reviews, 2));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(2);
            result.Value.TotalCount.Should().Be(2);
            result.Value.PageNumber.Should().Be(1);
            result.Value.PageSize.Should().Be(10);
            result.Value.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
        {
            var query = new GetCourseReviewsQuery(Guid.NewGuid(), 1, 10);

            _courseReviewQueryMock
                .Setup(x => x.GetAllAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<CourseReviewResponse>([], 0));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().BeEmpty();
            result.Value.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task Handle_MultiplePages_ComputesCorrectTotalPages()
        {
            var courseId = Guid.NewGuid();
            var query = new GetCourseReviewsQuery(courseId, 2, 10);
            var reviews = Enumerable.Range(1, 5)
                .Select(i => new CourseReviewResponse(
                    Guid.NewGuid(), courseId, Guid.NewGuid(), $"User {i}", 3, "OK", DateTimeOffset.UtcNow, null))
                .ToList();

            _courseReviewQueryMock
                .Setup(x => x.GetAllAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<CourseReviewResponse>(reviews, 25));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(5);
            result.Value.TotalCount.Should().Be(25);
            result.Value.PageNumber.Should().Be(2);
            result.Value.TotalPages.Should().Be(3);
        }
    }
}
