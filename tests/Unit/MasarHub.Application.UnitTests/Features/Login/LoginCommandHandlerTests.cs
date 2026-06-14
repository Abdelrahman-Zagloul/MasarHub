using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Account.Login;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Login
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
        private readonly LoginCommand _validCommand;
        private readonly TokenUser _tokenUser;
        private const string IpAddress = "127.0.0.1";

        public LoginCommandHandlerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _twoFactorChallengeStoreMock = new Mock<ITwoFactorChallengeStore>();

            _currentUserServiceMock.Setup(x => x.IpAddress).Returns(IpAddress);

            _sut = new LoginCommandHandler(
                _authServiceMock.Object,
                _currentUserServiceMock.Object,
                _tokenServiceMock.Object,
                _refreshTokenServiceMock.Object,
                _twoFactorChallengeStoreMock.Object);

            _validCommand = new LoginCommand("test@example.com", "Password123!");
            _tokenUser = new TokenUser(Guid.NewGuid(), "Test User", "test@example.com", ["Student"]);
        }

        [Fact]
        public async Task Handle_InvalidCredentials_ReturnsUnauthorizedError()
        {
            var error = Error.Unauthorized("auth.invalid_credentials");
            _authServiceMock
                .Setup(x => x.LoginAsync(_validCommand.Email, _validCommand.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<AuthenticateUserResult>.Failure(error));

            var result = await _sut.Handle(_validCommand, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "auth.invalid_credentials");
        }

        [Fact]
        public async Task Handle_TwoFactorRequired_ReturnsTwoFactorRequired()
        {
            var authResult = Result<AuthenticateUserResult>.Success(
                new AuthenticateUserResult(true, _tokenUser, TwoFactorProvider.Email));

            _authServiceMock
                .Setup(x => x.LoginAsync(_validCommand.Email, _validCommand.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(authResult);

            var challengeId = Guid.NewGuid();
            _twoFactorChallengeStoreMock
                .Setup(x => x.CreateAsync(_tokenUser.Id, TwoFactorProvider.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(challengeId);

            var result = await _sut.Handle(_validCommand, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.RequiresTwoFactor.Should().BeTrue();
            result.Value.ChallengeId.Should().Be(challengeId);
            result.Value.Provider.Should().Be(TwoFactorProvider.Email);
            result.Value.Tokens.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsSuccessWithTokens()
        {
            var authResult = Result<AuthenticateUserResult>.Success(
                new AuthenticateUserResult(false, _tokenUser, null));
            _authServiceMock
                .Setup(x => x.LoginAsync(_validCommand.Email, _validCommand.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(authResult);

            var accessTokenResponse = new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), _tokenUser.Id, ["Student"]);
            _tokenServiceMock
                .Setup(x => x.GenerateTokenAsync(_tokenUser))
                .ReturnsAsync(accessTokenResponse);

            var refreshTokenResult = new RefreshTokenResult("refresh-token", DateTimeOffset.UtcNow.AddDays(7), _tokenUser.Id);
            _refreshTokenServiceMock
                .Setup(x => x.CreateAsync(_tokenUser, IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RefreshTokenResult>.Success(refreshTokenResult));

            var result = await _sut.Handle(_validCommand, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.RequiresTwoFactor.Should().BeFalse();
            result.Value.Tokens.Should().NotBeNull();
            result.Value.Tokens!.AccessTokenResponse.Should().Be(accessTokenResponse);
            result.Value.Tokens!.RefreshTokenResult.Should().Be(refreshTokenResult);
        }

        [Fact]
        public async Task Handle_RefreshTokenCreationFails_ReturnsError()
        {
            var authResult = Result<AuthenticateUserResult>.Success(
                new AuthenticateUserResult(false, _tokenUser, null));
            _authServiceMock
                .Setup(x => x.LoginAsync(_validCommand.Email, _validCommand.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(authResult);

            var accessTokenResponse = new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), _tokenUser.Id, ["Student"]);
            _tokenServiceMock
                .Setup(x => x.GenerateTokenAsync(_tokenUser))
                .ReturnsAsync(accessTokenResponse);

            var error = Error.Failure("auth.refresh_token_failed");
            _refreshTokenServiceMock
                .Setup(x => x.CreateAsync(_tokenUser, IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RefreshTokenResult>.Failure(error));

            var result = await _sut.Handle(_validCommand, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "auth.refresh_token_failed");
        }
    }
}