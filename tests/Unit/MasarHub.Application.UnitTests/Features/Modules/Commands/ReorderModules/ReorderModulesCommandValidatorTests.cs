using FluentAssertions;
using MasarHub.Application.Features.Modules.Commands.ReorderModules;

namespace MasarHub.Application.UnitTests.Features.Modules.Commands.ReorderModules
{
    [Trait("UnitTests.Feature.Modules", "ReorderModules")]
    public sealed class ReorderModulesCommandValidatorTests
    {
        private readonly ReorderModulesCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new ReorderModulesCommand(Guid.NewGuid(), Guid.NewGuid(), [Guid.NewGuid(), Guid.NewGuid()]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var command = new ReorderModulesCommand(Guid.Empty, Guid.NewGuid(), [Guid.NewGuid()]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyCollection_ReturnsValidationError()
        {
            var command = new ReorderModulesCommand(Guid.NewGuid(), Guid.NewGuid(), []);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "OrderedModuleIds");
        }

        [Fact]
        public void Validate_DuplicateIds_ReturnsValidationError()
        {
            var id = Guid.NewGuid();
            var command = new ReorderModulesCommand(Guid.NewGuid(), Guid.NewGuid(), [id, id]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "module.duplicate_reorder_module_ids");
        }

        [Fact]
        public void Validate_ContainsEmptyGuid_ReturnsValidationError()
        {
            var command = new ReorderModulesCommand(Guid.NewGuid(), Guid.NewGuid(), [Guid.NewGuid(), Guid.Empty]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "module.invalid_module_ids");
        }
    }
}
