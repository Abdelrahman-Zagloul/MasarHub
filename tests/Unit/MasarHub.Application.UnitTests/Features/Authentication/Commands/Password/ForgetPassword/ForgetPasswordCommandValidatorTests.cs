using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.Password.ForgetPassword;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Password.ForgetPassword
{
    [Trait("UnitTests.Feature.Auth", "ForgetPassword")]
    public sealed class ForgetPasswordCommandValidatorTests
    {
        private readonly ForgetPasswordCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidEmail_ReturnsNoErrors()
        {
            var command = new ForgetPasswordCommand("abdelrahman@example.com");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("notanemail")]
        public void Validate_InvalidEmail_ReturnsValidationError(string email)
        {
            var command = new ForgetPasswordCommand(email);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }
    }
}
