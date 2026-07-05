using FluentAssertions;
using MasarHub.Application.Features.Progress.Queries.GetCourseProgress;

namespace MasarHub.Application.UnitTests.Features.Progress.Queries.GetCourseProgress
{
    [Trait("UnitTests.Feature.Progress", "GetCourseProgress")]
    public sealed class GetCourseProgressQueryValidatorTests
    {
        private readonly GetCourseProgressQueryValidator _sut = new();

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = new GetCourseProgressQuery(Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var query = new GetCourseProgressQuery(Guid.NewGuid(), Guid.Empty);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }
    }
}
