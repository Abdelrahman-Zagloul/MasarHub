using FluentAssertions;
using MasarHub.Application.Features.Modules.Queries.GetModuleById;

namespace MasarHub.Application.UnitTests.Features.Modules.Queries.GetModuleById
{
    [Trait("UnitTests.Feature.Modules", "GetModuleById")]
    public sealed class GetModuleByIdQueryValidatorTests
    {
        private readonly GetModuleByIdQueryValidator _sut = new();

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = new GetModuleByIdQuery(Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var query = new GetModuleByIdQuery(Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyModuleId_ReturnsValidationError()
        {
            var query = new GetModuleByIdQuery(Guid.NewGuid(), Guid.Empty);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ModuleId");
        }
    }
}
