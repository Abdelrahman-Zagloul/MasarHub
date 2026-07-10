using FluentAssertions;
using MasarHub.Application.Features.Courses.Queries.GetCourses;

namespace MasarHub.Application.UnitTests.Features.Courses.Queries.GetCourses
{
    [Trait("UnitTests.Feature.Courses", "GetCourses")]
    public sealed class GetCoursesQueryValidatorTests
    {
        private readonly GetCoursesQueryValidator _sut = new();

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = new GetCoursesQuery(null, null, null, null, null, null, null, 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCategoryId_ReturnsValidationError()
        {
            var query = new GetCoursesQuery(null, Guid.Empty, null, null, null, null, null, 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
        }

        [Fact]
        public void Validate_EmptyInstructorId_ReturnsValidationError()
        {
            var query = new GetCoursesQuery(null, null, Guid.Empty, null, null, null, null, 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "InstructorId");
        }

        [Fact]
        public void Validate_MaxPriceLessThanMinPrice_ReturnsValidationError()
        {
            var query = new GetCoursesQuery(null, null, null, null, null, 100, 50, 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
        }
    }
}
