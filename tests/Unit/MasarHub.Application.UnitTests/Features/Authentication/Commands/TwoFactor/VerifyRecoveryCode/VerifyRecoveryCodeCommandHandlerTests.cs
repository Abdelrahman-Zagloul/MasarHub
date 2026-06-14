using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyRecoveryCode;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyRecoveryCode.Events;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.TwoFactor.VerifyRecoveryCode
{
    [Trait("UnitTests", "Feature.Auth.VerifyRecoveryCode.Handler")]
    public sealed class VerifyRecoveryCodeCommandHandlerTests
    {
        private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly VerifyRecoveryCodeCommandHandler _sut;
        private const string IpAddress = "10.0.0.1";
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly TokenUser User = new(UserId, "Abdelrahman", "abdelrahman@example.com", ["Student"]);

        public VerifyRecoveryCodeCommandHandlerTests()
        {
            _twoFactorServiceMock = new Mock<ITwoFactorService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _mediatorMock = new Mock<IMediator>();
            _currentUserServiceMock.Setup(x => x.IpAddress).Returns(IpAddress);
            _sut = new VerifyRecoveryCodeCommandHandler(_twoFactorServiceMock.Object, _tokenServiceMock.Object, _refreshTokenServiceMock.Object, _currentUserServiceMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCode_ReturnsTokensAndPublishesEvent()
        {
            var challengeId = Guid.NewGuid();
            var command = new VerifyRecoveryCodeCommand(challengeId, "correct-code");

            _twoFactorServiceMock
                .Setup(x => x.VerifyRecoveryCodeAsync(challengeId, "correct-code"))
                .ReturnsAsync(Result<TokenUser>.Success(User));
            _tokenServiceMock
                .Setup(x => x.GenerateTokenAsync(User))
                .ReturnsAsync(new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), UserId, ["Student"]));
            _refreshTokenServiceMock
                .Setup(x => x.CreateAsync(User, IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RefreshTokenResult>.Success(new RefreshTokenResult("refresh-token", DateTimeOffset.UtcNow.AddDays(7), UserId)));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.IsAny<TwoFactorRecoveryCodeUsedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCode_ReturnsError()
        {
            var challengeId = Guid.NewGuid();
            var command = new VerifyRecoveryCodeCommand(challengeId, "wrong-code");

            _twoFactorServiceMock
                .Setup(x => x.VerifyRecoveryCodeAsync(challengeId, "wrong-code"))
                .ReturnsAsync(Error.BadRequest("2fa.invalid_recovery_code"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
    }
}
