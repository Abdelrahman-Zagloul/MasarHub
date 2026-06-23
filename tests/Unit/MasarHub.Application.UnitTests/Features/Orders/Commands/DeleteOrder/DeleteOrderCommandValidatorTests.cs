using FluentAssertions;
using MasarHub.Application.Features.Orders.Commands.DeleteOrder;

namespace MasarHub.Application.UnitTests.Features.Orders.Commands.DeleteOrder
{
    [Trait("UnitTests.Feature.Orders", "DeleteOrder")]
    public sealed class DeleteOrderCommandValidatorTests
    {
        private readonly DeleteOrderCommandValidator _sut = new();

        [Fact]
        public void Validate_EmptyOrderId_HasValidationError()
        {
            var command = new DeleteOrderCommand(Guid.NewGuid(), Guid.Empty);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
        }

        [Fact]
        public void Validate_ValidCommand_NoValidationErrors()
        {
            var command = new DeleteOrderCommand(Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
