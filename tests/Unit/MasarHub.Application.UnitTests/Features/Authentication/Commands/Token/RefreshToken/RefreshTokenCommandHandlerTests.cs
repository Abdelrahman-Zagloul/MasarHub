using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Token.RefreshToken;
using MasarHub.Application.Features.Authentication.Shared;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Token.RefreshToken
{
    [Trait("UnitTests", "Feature.Auth.RefreshToken.Handler")]
    public sealed class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly Mock<ITokenService> _accessTokenServiceMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly RefreshTokenCommandHandler _sut;
        private const string IpAddress = "10.0.0.1";
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly TokenUser User = new(UserId, "Abdelrahman", "abdelrahman@example.com", ["Student"]);

        public RefreshTokenCommandHandlerTests()
        {
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _accessTokenServiceMock = new Mock<ITokenService>();
            _authServiceMock = new Mock<IAuthService>();
            _sut = new RefreshTokenCommandHandler(_refreshTokenServiceMock.Object, _accessTokenServiceMock.Object, _authServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidToken_ReturnsNewTokens()
        {
            var command = new RefreshTokenCommand("valid-refresh-token", IpAddress);
            var rotatedToken = new RefreshTokenResult("new-refresh-token", DateTimeOffset.UtcNow.AddDays(7), UserId);

            _refreshTokenServiceMock
                .Setup(x => x.RotateAsync("valid-refresh-token", IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RefreshTokenResult>.Success(rotatedToken));
            _authServiceMock
                .Setup(x => x.GetUserAsync(UserId))
                .ReturnsAsync(Result<TokenUser>.Success(User));
            _accessTokenServiceMock
                .Setup(x => x.GenerateTokenAsync(User))
                .ReturnsAsync(new AccessTokenResponse("new-access-token", DateTime.UtcNow.AddHours(1), UserId, ["Student"]));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_MissingToken_ReturnsUnauthorized()
        {
            var command = new RefreshTokenCommand(null, IpAddress);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ExpiredToken_ReturnsError()
        {
            var command = new RefreshTokenCommand("expired-token", IpAddress);
            _refreshTokenServiceMock
                .Setup(x => x.RotateAsync("expired-token", IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.Unauthorized("auth.refresh_token_expired"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
    }
}
