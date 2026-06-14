using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.SendTwoFactorCode;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.TwoFactor.SendTwoFactorCode
{
    [Trait("UnitTests", "Feature.Auth.SendTwoFactorCode.Validator")]
    public sealed class SendTwoFactorCodeCommandValidatorTests
    {
        private readonly SendTwoFactorCodeCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidChallengeId_ReturnsNoErrors()
        {
            var command = new SendTwoFactorCodeCommand(Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
