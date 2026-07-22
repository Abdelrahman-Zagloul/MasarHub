using FluentAssertions;
using FluentValidation.TestHelper;
using MasarHub.Application.Features.Reviews.Queries.GetCourseReviews;

namespace MasarHub.Application.UnitTests.Features.Reviews.Queries.GetCourseReviews
{
    [Trait("UnitTests.Feature.Reviews", "GetCourseReviews")]
    public sealed class GetCourseReviewsQueryValidatorTests
    {
        private readonly GetCourseReviewsQueryValidator _sut = new();

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = new GetCourseReviewsQuery(Guid.NewGuid(), 1, 10);

            var result = _sut.TestValidate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var query = new GetCourseReviewsQuery(Guid.Empty, 1, 10);

            var result = _sut.TestValidate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_InvalidPageNumber_ReturnsError(int pageNumber)
        {
            var query = new GetCourseReviewsQuery(Guid.NewGuid(), pageNumber, 10);

            var result = _sut.TestValidate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PageNumber");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(51)]
        public void Validate_InvalidPageSize_ReturnsError(int pageSize)
        {
            var query = new GetCourseReviewsQuery(Guid.NewGuid(), 1, pageSize);

            var result = _sut.TestValidate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
        }
    }
}
