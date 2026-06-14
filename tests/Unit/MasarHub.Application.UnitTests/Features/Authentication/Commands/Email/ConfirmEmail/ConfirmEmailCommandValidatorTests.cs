using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.Email.ConfirmEmail;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Email.ConfirmEmail
{
    [Trait("UnitTests", "Feature.Auth.ConfirmEmail.Validator")]
    public sealed class ConfirmEmailCommandValidatorTests
    {
        private readonly ConfirmEmailCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new ConfirmEmailCommand("abdelrahman@example.com", "some-token");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-an-email")]
        public void Validate_InvalidEmail_ReturnsValidationError(string email)
        {
            var command = new ConfirmEmailCommand(email, "some-token");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_InvalidToken_ReturnsValidationError(string token)
        {
            var command = new ConfirmEmailCommand("abdelrahman@example.com", token);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Token");
        }
    }
}
