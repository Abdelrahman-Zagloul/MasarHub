using FluentAssertions;
using MasarHub.Application.Features.Payments.Commands.InitiatePayment;
using MasarHub.Domain.Modules.Payments;

namespace MasarHub.Application.UnitTests.Features.Payments.Commands.InitiatePayment
{
    [Trait("UnitTests.Feature.Payments", "InitiatePayment")]
    public sealed class InitiatePaymentCommandValidatorTests
    {
        private readonly InitiatePaymentCommandValidator _sut = new();

        [Fact]
        public void Validate_EmptyUserId_HasValidationError()
        {
            var command = new InitiatePaymentCommand(Guid.Empty, Guid.NewGuid(), PaymentProvider.Stripe);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "UserId");
        }

        [Fact]
        public void Validate_EmptyOrderId_HasValidationError()
        {
            var command = new InitiatePaymentCommand(Guid.NewGuid(), Guid.Empty, PaymentProvider.Stripe);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
        }

        [Fact]
        public void Validate_InvalidProvider_HasValidationError()
        {
            var command = new InitiatePaymentCommand(Guid.NewGuid(), Guid.NewGuid(), (PaymentProvider)999);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Provider");
        }

        [Fact]
        public void Validate_ValidCommand_NoValidationErrors()
        {
            var command = new InitiatePaymentCommand(Guid.NewGuid(), Guid.NewGuid(), PaymentProvider.Stripe);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
