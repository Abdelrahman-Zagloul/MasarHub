using FluentAssertions;
using MasarHub.Application.Features.Courses.Queries.GetCourseById;

namespace MasarHub.Application.UnitTests.Features.Courses.Queries.GetCourseById
{
    [Trait("UnitTests.Feature.Courses", "GetCourseById")]
    public sealed class GetCourseByIdQueryValidatorTests
    {
        private readonly GetCourseByIdQueryValidator _sut = new();

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = new GetCourseByIdQuery(Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyId_ReturnsValidationError()
        {
            var query = new GetCourseByIdQuery(Guid.Empty);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Id");
        }
    }
}
