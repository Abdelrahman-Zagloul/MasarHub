using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.Account.Login;

namespace MasarHub.Application.UnitTests.Features.Login;

[Trait("UnitTests", "Feature.Auth.Login.Validator")]
public sealed class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _sut = new();

    [Fact]
    public void Validate_ValidLoginCommand_ReturnsNoErrors()
    {
        var command = new LoginCommand("user@example.com", "MyPassword123!");

        var result = _sut.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]             // Empty
    [InlineData("plaintext")]    // Missing @
    [InlineData("@domain.com")]  // Missing local part
    public void Validate_InvalidEmail_ReturnsValidationError(string email)
    {
        var command = new LoginCommand(email, "MyPassword123!");

        var result = _sut.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]     // Empty
    [InlineData("   ")]  // Whitespace only
    public void Validate_InvalidPassword_ReturnsValidationError(string password)
    {
        var command = new LoginCommand("user@example.com", password);

        var result = _sut.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_AllFieldsEmpty_ReturnsMultipleErrors()
    {
        var command = new LoginCommand("", "");

        var result = _sut.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
