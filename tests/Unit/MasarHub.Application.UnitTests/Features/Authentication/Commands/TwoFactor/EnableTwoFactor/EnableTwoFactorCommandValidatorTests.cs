using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.TwoFactor.EnableTwoFactor
{
    [Trait("UnitTests", "Feature.Auth.EnableTwoFactor.Validator")]
    public sealed class EnableTwoFactorCommandValidatorTests
    {
        private readonly EnableTwoFactorCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidProvider_ReturnsNoErrors()
        {
            var command = new EnableTwoFactorCommand(TwoFactorProvider.Authenticator);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_InvalidProvider_ReturnsValidationError()
        {
            var command = new EnableTwoFactorCommand((TwoFactorProvider)99);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Provider");
        }
    }
}
