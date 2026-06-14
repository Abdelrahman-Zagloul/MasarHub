using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyTwoFactorCode;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.TwoFactor.VerifyTwoFactorCode
{
    [Trait("UnitTests", "Feature.Auth.VerifyTwoFactorCode.Validator")]
    public sealed class VerifyTwoFactorCodeCommandValidatorTests
    {
        private readonly VerifyTwoFactorCodeCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new VerifyTwoFactorCodeCommand(Guid.NewGuid(), "123456");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_InvalidChallengeId_ReturnsValidationError()
        {
            var command = new VerifyTwoFactorCodeCommand(Guid.Empty, "123456");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ChallengeId");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("12")]
        [InlineData("abcdef")]
        public void Validate_InvalidCode_ReturnsValidationError(string code)
        {
            var command = new VerifyTwoFactorCodeCommand(Guid.NewGuid(), code);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Code");
        }
    }
}
