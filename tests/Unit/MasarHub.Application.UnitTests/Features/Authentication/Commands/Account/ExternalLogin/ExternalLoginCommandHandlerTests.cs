using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin;
using MasarHub.Application.Features.Authentication.Commands.Email.ConfirmEmail.Events;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Account.ExternalLogin
{
    [Trait("UnitTests", "Feature.Auth.ExternalLogin.Handler")]
    public sealed class ExternalLoginCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IExternalAuthService> _externalAuthServiceMock;
        private readonly Mock<IExternalAuthProvider> _providerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly ExternalLoginCommandHandler _sut;
        private const string IpAddress = "10.0.0.1";
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly TokenUser TokenUser = new(UserId, "Abdelrahman", "abdelrahman@example.com", ["Student"]);

        public ExternalLoginCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _externalAuthServiceMock = new Mock<IExternalAuthService>();
            _providerMock = new Mock<IExternalAuthProvider>();
            _tokenServiceMock = new Mock<ITokenService>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _currentUserServiceMock.Setup(x => x.IpAddress).Returns(IpAddress);
            _sut = new ExternalLoginCommandHandler(
                _mediatorMock.Object, _externalAuthServiceMock.Object,
                [_providerMock.Object], _tokenServiceMock.Object,
                _refreshTokenServiceMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_NewUser_ReturnsTokensAndPublishesEvent()
        {
            var userInfo = new ExternalUserInfo("abdelrahman@example.com", "Abdelrahman", "google-id", ExternalLoginProvider.Google);
            var command = new ExternalLoginCommand(ExternalLoginProvider.Google, "google-token");

            _providerMock.Setup(x => x.Provider).Returns(ExternalLoginProvider.Google);
            _providerMock
                .Setup(x => x.VerifyAsync("google-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ExternalUserInfo>.Success(userInfo));
            _externalAuthServiceMock
                .Setup(x => x.LoginAsync(userInfo))
                .ReturnsAsync(Result<ExternalLoginResult>.Success(new ExternalLoginResult(TokenUser, "Abdelrahman", IsNew: true)));
            _tokenServiceMock
                .Setup(x => x.GenerateTokenAsync(TokenUser))
                .ReturnsAsync(new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), UserId, ["Student"]));
            _refreshTokenServiceMock
                .Setup(x => x.CreateAsync(TokenUser, IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RefreshTokenResult>.Success(new RefreshTokenResult("refresh-token", DateTimeOffset.UtcNow.AddDays(7), UserId)));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.IsAny<EmailConfirmedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ExistingUser_ReturnsTokensWithoutEvent()
        {
            var userInfo = new ExternalUserInfo("abdelrahman@example.com", "Abdelrahman", "google-id", ExternalLoginProvider.Google);
            var command = new ExternalLoginCommand(ExternalLoginProvider.Google, "google-token");

            _providerMock.Setup(x => x.Provider).Returns(ExternalLoginProvider.Google);
            _providerMock
                .Setup(x => x.VerifyAsync("google-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ExternalUserInfo>.Success(userInfo));
            _externalAuthServiceMock
                .Setup(x => x.LoginAsync(userInfo))
                .ReturnsAsync(Result<ExternalLoginResult>.Success(new ExternalLoginResult(TokenUser, "Abdelrahman", IsNew: false)));
            _tokenServiceMock
                .Setup(x => x.GenerateTokenAsync(TokenUser))
                .ReturnsAsync(new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), UserId, ["Student"]));
            _refreshTokenServiceMock
                .Setup(x => x.CreateAsync(TokenUser, IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RefreshTokenResult>.Success(new RefreshTokenResult("refresh-token", DateTimeOffset.UtcNow.AddDays(7), UserId)));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.IsAny<EmailConfirmedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ProviderVerificationFails_ReturnsError()
        {
            var command = new ExternalLoginCommand(ExternalLoginProvider.Google, "bad-token");
            _providerMock.Setup(x => x.Provider).Returns(ExternalLoginProvider.Google);
            _providerMock
                .Setup(x => x.VerifyAsync("bad-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.BadRequest("external_auth.invalid_token"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_LoginFails_ReturnsError()
        {
            var userInfo = new ExternalUserInfo("abdelrahman@example.com", "Abdelrahman", "google-id", ExternalLoginProvider.Google);
            var command = new ExternalLoginCommand(ExternalLoginProvider.Google, "valid-token");

            _providerMock.Setup(x => x.Provider).Returns(ExternalLoginProvider.Google);
            _providerMock
                .Setup(x => x.VerifyAsync("valid-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ExternalUserInfo>.Success(userInfo));
            _externalAuthServiceMock
                .Setup(x => x.LoginAsync(userInfo))
                .ReturnsAsync(Error.BadRequest("external_auth.login_failed"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
        [Fact]
        public async Task Handle_RefreshTokenFails_ReturnsError()
        {
            var userInfo = new ExternalUserInfo("abdelrahman@example.com", "Abdelrahman", "google-id", ExternalLoginProvider.Google);
            var command = new ExternalLoginCommand(ExternalLoginProvider.Google, "google-token");

            _providerMock.Setup(x => x.Provider).Returns(ExternalLoginProvider.Google);
            _providerMock
                .Setup(x => x.VerifyAsync("google-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ExternalUserInfo>.Success(userInfo));

            _externalAuthServiceMock
                .Setup(x => x.LoginAsync(userInfo))
                .ReturnsAsync(Result<ExternalLoginResult>.Success(new ExternalLoginResult(TokenUser, "Abdelrahman", IsNew: false)));

            _tokenServiceMock
                .Setup(x => x.GenerateTokenAsync(TokenUser))
                .ReturnsAsync(new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), UserId, ["Student"]));

            _refreshTokenServiceMock
                .Setup(x => x.CreateAsync(TokenUser, IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.Failure("auth.refresh_token_failed"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "auth.refresh_token_failed");
        }
    }
}
