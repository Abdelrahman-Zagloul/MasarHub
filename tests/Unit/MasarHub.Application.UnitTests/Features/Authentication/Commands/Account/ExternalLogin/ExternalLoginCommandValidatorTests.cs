using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Account.ExternalLogin
{
    [Trait("UnitTests.Feature.Auth", "ExternalLogin")]
    public sealed class ExternalLoginCommandValidatorTests
    {
        private readonly ExternalLoginCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new ExternalLoginCommand(ExternalLoginProvider.Google, "valid-token");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_InvalidProvider_ReturnsValidationError()
        {
            var command = new ExternalLoginCommand((ExternalLoginProvider)99, "valid-token");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Provider");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_InvalidToken_ReturnsValidationError(string token)
        {
            var command = new ExternalLoginCommand(ExternalLoginProvider.Google, token);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Token");
        }
    }
}
