using FluentAssertions;
using MasarHub.Application.Features.Orders.Commands.CancelOrder;

namespace MasarHub.Application.UnitTests.Features.Orders.Commands.CancelOrder
{
    [Trait("UnitTests.Feature.Orders", "CancelOrder")]
    public sealed class CancelOrderCommandValidatorTests
    {
        private readonly CancelOrderCommandValidator _sut = new();

        [Fact]
        public void Validate_EmptyOrderId_HasValidationError()
        {
            var command = new CancelOrderCommand(Guid.NewGuid(), Guid.Empty);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
        }

        [Fact]
        public void Validate_ValidCommand_NoValidationErrors()
        {
            var command = new CancelOrderCommand(Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
