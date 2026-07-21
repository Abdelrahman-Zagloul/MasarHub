using FluentAssertions;
using FluentValidation.TestHelper;
using MasarHub.Application.Features.Reviews.Queries.GetCourseReviewById;

namespace MasarHub.Application.UnitTests.Features.Reviews.Queries.GetCourseReviewById
{
    [Trait("UnitTests.Feature.Reviews", "GetCourseReviewById")]
    public sealed class GetCourseReviewByIdQueryValidatorTests
    {
        private readonly GetCourseReviewByIdQueryValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var query = new GetCourseReviewByIdQuery(Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.TestValidate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var query = new GetCourseReviewByIdQuery(Guid.Empty, Guid.NewGuid());

            var result = _sut.TestValidate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyReviewId_ReturnsError()
        {
            var query = new GetCourseReviewByIdQuery(Guid.NewGuid(), Guid.Empty);

            var result = _sut.TestValidate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ReviewId");
        }
    }
}
