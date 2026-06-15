using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.SetupAuthenticator;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.TwoFactor.SetupAuthenticator
{
    [Trait("UnitTests.Feature.Auth", "SetupAuthenticator")]
    public sealed class SetupAuthenticatorCommandHandlerTests
    {
        private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
        private readonly SetupAuthenticatorCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();

        public SetupAuthenticatorCommandHandlerTests()
        {
            _twoFactorServiceMock = new Mock<ITwoFactorService>();
            _sut = new SetupAuthenticatorCommandHandler(_twoFactorServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsAuthenticatorSetupData()
        {
            var command = new SetupAuthenticatorCommand(UserId);
            var expected = new SetupAuthenticatorResult("shared-key", "qr-uri");
            _twoFactorServiceMock
                .Setup(x => x.SetupAuthenticatorAsync(UserId))
                .ReturnsAsync(Result<SetupAuthenticatorResult>.Success(expected));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.SharedKey.Should().Be("shared-key");
            result.Value.AuthenticatorUri.Should().Be("qr-uri");
        }
    }
}
