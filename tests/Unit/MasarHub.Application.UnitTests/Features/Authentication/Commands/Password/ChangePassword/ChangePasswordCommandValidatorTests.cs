using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Password.ChangePassword
{
    [Trait("UnitTests.Feature.Auth", "ChangePassword")]
    public sealed class ChangePasswordCommandValidatorTests
    {
        private readonly ChangePasswordCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new ChangePasswordCommand(Guid.NewGuid(), "CurrentPass1!", "NewPass123!");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("short")]
        public void Validate_InvalidCurrentPassword_ReturnsValidationError(string currentPassword)
        {
            var command = new ChangePasswordCommand(Guid.NewGuid(), currentPassword, "NewPass123!");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CurrentPassword" || e.PropertyName == "Current Password");
        }

        [Theory]
        [InlineData("")]
        [InlineData("short")]
        public void Validate_InvalidNewPassword_ReturnsValidationError(string newPassword)
        {
            var command = new ChangePasswordCommand(Guid.NewGuid(), "CurrentPass1!", newPassword);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "NewPassword" || e.PropertyName == "New Password");
        }
    }
}
