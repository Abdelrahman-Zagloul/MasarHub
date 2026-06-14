using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Account.Login;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Account.Login
{
    [Trait("UnitTests", "Feature.Auth.Login.Handler")]
    public sealed class LoginCommandHandlerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly Mock<ITwoFactorChallengeStore> _twoFactorChallengeStoreMock;
        private readonly LoginCommandHandler _sut;
        private const string IpAddress = "192.168.1.1";
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly TokenUser User = new(UserId, "Abdelrahman", "abdelrahman@example.com", ["Student"]);

        public LoginCommandHandlerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _twoFactorChallengeStoreMock = new Mock<ITwoFactorChallengeStore>();
            _currentUserServiceMock.Setup(x => x.IpAddress).Returns(IpAddress);
            _sut = new LoginCommandHandler(_authServiceMock.Object, _currentUserServiceMock.Object, _tokenServiceMock.Object, _refreshTokenServiceMock.Object, _twoFactorChallengeStoreMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsSuccessWithTokens()
        {
            var command = new LoginCommand("abdelrahman@example.com", "CorrectPass123!");
            var authResult = new AuthenticateUserResult(RequiresTwoFactor: false, User, Provider: null);
            _authServiceMock
                .Setup(x => x.LoginAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<AuthenticateUserResult>.Success(authResult));

            var accessToken = new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), UserId, ["Student"]);
            _tokenServiceMock.Setup(x => x.GenerateTokenAsync(User)).ReturnsAsync(accessToken);

            var refreshToken = new RefreshTokenResult("refresh-token", DateTimeOffset.UtcNow.AddDays(7), UserId);
            _refreshTokenServiceMock
                .Setup(x => x.CreateAsync(User, IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RefreshTokenResult>.Success(refreshToken));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.RequiresTwoFactor.Should().BeFalse();
            result.Value.Tokens.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_InvalidCredentials_ReturnsUnauthorized()
        {
            var command = new LoginCommand("abdelrahman@example.com", "WrongPass!");
            _authServiceMock
                .Setup(x => x.LoginAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.Unauthorized("auth.invalid_credentials"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_TwoFactorRequired_ReturnsTwoFactorChallenge()
        {
            var command = new LoginCommand("abdelrahman@example.com", "CorrectPass123!");
            var authResult = new AuthenticateUserResult(RequiresTwoFactor: true, User, Provider: TwoFactorProvider.Authenticator);
            _authServiceMock
                .Setup(x => x.LoginAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<AuthenticateUserResult>.Success(authResult));

            var challengeId = Guid.NewGuid();
            _twoFactorChallengeStoreMock
                .Setup(x => x.CreateAsync(UserId, TwoFactorProvider.Authenticator, It.IsAny<CancellationToken>()))
                .ReturnsAsync(challengeId);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.RequiresTwoFactor.Should().BeTrue();
            result.Value.ChallengeId.Should().Be(challengeId);
            result.Value.Tokens.Should().BeNull();
        }

        [Fact]
        public async Task Handle_RefreshTokenGenerationFails_ReturnsFailure()
        {
            var command = new LoginCommand("abdelrahman@example.com", "CorrectPass123!");
            var authResult = new AuthenticateUserResult(RequiresTwoFactor: false, User, Provider: null);

            _authServiceMock
                .Setup(x => x.LoginAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<AuthenticateUserResult>.Success(authResult));
            _tokenServiceMock
                .Setup(x => x.GenerateTokenAsync(User))
                .ReturnsAsync(new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), UserId, ["Student"]));
            _refreshTokenServiceMock
                .Setup(x => x.CreateAsync(User, IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.Failure("auth.refresh_token_failed"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "auth.refresh_token_failed");
        }
    }
}
