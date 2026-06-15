using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.Password.ResetPassword;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Password.ResetPassword
{
    [Trait("UnitTests.Feature.Auth", "ResetPassword")]
    public sealed class ResetPasswordCommandValidatorTests
    {
        private readonly ResetPasswordCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new ResetPasswordCommand("abdelrahman@example.com", "some-token", "NewPass123!");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-an-email")]
        public void Validate_InvalidEmail_ReturnsValidationError(string email)
        {
            var command = new ResetPasswordCommand(email, "some-token", "NewPass123!");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_InvalidToken_ReturnsValidationError(string token)
        {
            var command = new ResetPasswordCommand("abdelrahman@example.com", token, "NewPass123!");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Token");
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        public void Validate_InvalidNewPassword_ReturnsValidationError(string newPassword)
        {
            var command = new ResetPasswordCommand("abdelrahman@example.com", "some-token", newPassword);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "NewPassword");
        }
    }
}
