using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.Email.ResendConfirmEmail;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Email.ResendConfirmEmail
{
    [Trait("UnitTests.Feature.Auth", "ResendConfirmEmail")]
    public sealed class ResendConfirmEmailCommandValidatorTests
    {
        private readonly ResendConfirmEmailCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidEmail_ReturnsNoErrors()
        {
            var command = new ResendConfirmEmailCommand("abdelrahman@example.com");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("notanemail")]
        public void Validate_InvalidEmail_ReturnsValidationError(string email)
        {
            var command = new ResendConfirmEmailCommand(email);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }
    }
}
