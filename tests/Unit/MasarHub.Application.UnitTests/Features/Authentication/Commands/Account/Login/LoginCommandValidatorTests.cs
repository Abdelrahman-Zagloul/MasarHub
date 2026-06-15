using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.Account.Login;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Account.Login
{
    [Trait("UnitTests.Feature.Auth", "Login")]
    public sealed class LoginCommandValidatorTests
    {
        private readonly LoginCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new LoginCommand("abdelrahman@example.com", "SecurePass123!");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("not-email")]
        public void Validate_InvalidEmail_ReturnsValidationError(string email)
        {
            var command = new LoginCommand(email, "SecurePass123!");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_InvalidPassword_ReturnsValidationError(string password)
        {
            var command = new LoginCommand("abdelrahman@example.com", password);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }
    }
}
