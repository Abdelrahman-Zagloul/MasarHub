using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyAuthenticator;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.TwoFactor.VerifyAuthenticator
{
    [Trait("UnitTests.Feature.Auth", "VerifyAuthenticator")]
    public sealed class VerifyAuthenticatorCommandValidatorTests
    {
        private readonly VerifyAuthenticatorCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCode_ReturnsNoErrors()
        {
            var command = new VerifyAuthenticatorCommand("123456");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("12")]
        [InlineData("abcdef")]
        public void Validate_InvalidCode_ReturnsValidationError(string code)
        {
            var command = new VerifyAuthenticatorCommand(code);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Code");
        }
    }
}
