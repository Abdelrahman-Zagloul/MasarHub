using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Email.ConfirmEmail;
using MasarHub.Application.Features.Authentication.Commands.Email.ConfirmEmail.Events;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Email.ConfirmEmail
{
    [Trait("UnitTests.Feature.Auth", "ConfirmEmail")]
    public sealed class ConfirmEmailCommandHandlerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly ConfirmEmailCommandHandler _sut;
        private const string IpAddress = "10.0.0.1";
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly TokenUser User = new(UserId, "Abdelrahman", "abdelrahman@example.com", ["Student"]);

        public ConfirmEmailCommandHandlerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _mediatorMock = new Mock<IMediator>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _currentUserServiceMock.Setup(x => x.IpAddress).Returns(IpAddress);
            _sut = new ConfirmEmailCommandHandler(_authServiceMock.Object, _tokenServiceMock.Object, _refreshTokenServiceMock.Object, _mediatorMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidConfirmation_ReturnsTokensAndPublishesEvent()
        {
            var command = new ConfirmEmailCommand("abdelrahman@example.com", "encoded-token");

            _authServiceMock
                .Setup(x => x.ConfirmEmailAsync(command.Email, "encoded-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<TokenUser>.Success(User));
            _tokenServiceMock
                .Setup(x => x.GenerateTokenAsync(User))
                .ReturnsAsync(new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), UserId, ["Student"]));
            _refreshTokenServiceMock
                .Setup(x => x.CreateAsync(User, IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RefreshTokenResult>.Success(new RefreshTokenResult("refresh-token", DateTimeOffset.UtcNow.AddDays(7), UserId)));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.IsAny<EmailConfirmedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidToken_ReturnsError()
        {
            var command = new ConfirmEmailCommand("abdelrahman@example.com", "wrong-token");

            _authServiceMock
                .Setup(x => x.ConfirmEmailAsync(command.Email, "wrong-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.BadRequest("auth.invalid_confirmation_token"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
    }
}
