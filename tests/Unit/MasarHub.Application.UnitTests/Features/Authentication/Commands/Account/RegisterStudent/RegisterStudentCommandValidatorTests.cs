using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.Account.RegisterStudent;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Account.RegisterStudent
{
    [Trait("UnitTests.Feature.Auth", "RegisterStudent")]
    public sealed class RegisterStudentCommandValidatorTests
    {
        private readonly RegisterStudentCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new RegisterStudentCommand("Abdelrahman", "abdelrahman@example.com", "01009876543", Gender.Male, "SecurePass456!");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_InvalidFullName_ReturnsValidationError(string fullName)
        {
            var command = new RegisterStudentCommand(fullName, "abdelrahman@example.com", "01009876543", Gender.Male, "SecurePass456!");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "FullName");
        }

        [Theory]
        [InlineData("")]
        [InlineData("notanemail")]
        public void Validate_InvalidEmail_ReturnsValidationError(string email)
        {
            var command = new RegisterStudentCommand("Abdelrahman", email, "01009876543", Gender.Male, "SecurePass456!");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Theory]
        [InlineData("")]
        [InlineData("12345")]
        public void Validate_InvalidPhoneNumber_ReturnsValidationError(string phone)
        {
            var command = new RegisterStudentCommand("Abdelrahman", "abdelrahman@example.com", phone, Gender.Male, "SecurePass456!");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PhoneNumber");
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        public void Validate_InvalidPassword_ReturnsValidationError(string password)
        {
            var command = new RegisterStudentCommand("Abdelrahman", "abdelrahman@example.com", "01009876543", Gender.Male, password);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }
    }
}
