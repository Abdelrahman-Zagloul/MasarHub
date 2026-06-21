using MasarHub.Application.Features.Carts.Commands.RemoveFromCart;
using FluentAssertions;

namespace MasarHub.Application.UnitTests.Features.Carts.Commands.RemoveFromCart
{
    [Trait("UnitTests.Feature.Carts", "RemoveFromCart")]
    public sealed class RemoveFromCartCommandValidatorTests
    {
        private readonly RemoveFromCartCommandValidator _sut = new();

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var command = new RemoveFromCartCommand(Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new RemoveFromCartCommand(Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}