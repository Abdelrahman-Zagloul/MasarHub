using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.Password.VerifyPassword;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Password.VerifyPassword
{
    [Trait("UnitTests", "Feature.Auth.VerifyPassword.Validator")]
    public sealed class VerifyPasswordCommandValidatorTests
    {
        private readonly VerifyPasswordCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidPassword_ReturnsNoErrors()
        {
            var command = new VerifyPasswordCommand("ValidPass123!");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_InvalidPassword_ReturnsValidationError(string password)
        {
            var command = new VerifyPasswordCommand(password);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }
    }
}
