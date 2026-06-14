using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyTwoFactorCode;
using MasarHub.Application.Features.Authentication.Shared;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.TwoFactor.VerifyTwoFactorCode
{
    [Trait("UnitTests", "Feature.Auth.VerifyTwoFactorCode.Handler")]
    public sealed class VerifyTwoFactorCodeCommandHandlerTests
    {
        private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly VerifyTwoFactorCodeCommandHandler _sut;
        private const string IpAddress = "10.0.0.1";
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly TokenUser User = new(UserId, "Abdelrahman", "abdelrahman@example.com", ["Student"]);

        public VerifyTwoFactorCodeCommandHandlerTests()
        {
            _twoFactorServiceMock = new Mock<ITwoFactorService>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _currentUserServiceMock.Setup(x => x.IpAddress).Returns(IpAddress);
            _sut = new VerifyTwoFactorCodeCommandHandler(_twoFactorServiceMock.Object, _refreshTokenServiceMock.Object, _tokenServiceMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCode_ReturnsTokens()
        {
            var challengeId = Guid.NewGuid();
            var command = new VerifyTwoFactorCodeCommand(challengeId, "123456");

            _twoFactorServiceMock
                .Setup(x => x.VerifyCodeAsync(challengeId, "123456", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<TokenUser>.Success(User));
            _tokenServiceMock
                .Setup(x => x.GenerateTokenAsync(User))
                .ReturnsAsync(new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), UserId, ["Student"]));
            _refreshTokenServiceMock
                .Setup(x => x.CreateAsync(User, IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RefreshTokenResult>.Success(new RefreshTokenResult("refresh-token", DateTimeOffset.UtcNow.AddDays(7), UserId)));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_InvalidCode_ReturnsError()
        {
            var challengeId = Guid.NewGuid();
            var command = new VerifyTwoFactorCodeCommand(challengeId, "wrong-code");

            _twoFactorServiceMock
                .Setup(x => x.VerifyCodeAsync(challengeId, "wrong-code", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.BadRequest("2fa.invalid_code"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
    }
}
