using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword;
using MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword.Events;
using MasarHub.Application.Features.Authentication.Commands.Password.ResetPassword;
using MediatR;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Password.ResetPassword
{
    [Trait("UnitTests.Feature.Auth", "ResetPassword")]
    public sealed class ResetPasswordCommandHandlerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly ResetPasswordCommandHandler _sut;
        private const string IpAddress = "10.0.0.1";

        public ResetPasswordCommandHandlerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _mediatorMock = new Mock<IMediator>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _currentUserServiceMock.Setup(x => x.IpAddress).Returns(IpAddress);
            _sut = new ResetPasswordCommandHandler(_authServiceMock.Object, _mediatorMock.Object, _refreshTokenServiceMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidReset_ReturnsSuccessAndRevokesTokens()
        {
            var userId = Guid.NewGuid();
            var command = new ResetPasswordCommand("abdelrahman@example.com", "valid-token", "NewPass123!");
            var passwordResult = new PasswordChangedResult(userId, "Abdelrahman", "abdelrahman@example.com");
            _authServiceMock
                .Setup(x => x.ResetPasswordAsync(command.Email, "valid-token", command.NewPassword))
                .ReturnsAsync(Result<PasswordChangedResult>.Success(passwordResult));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _refreshTokenServiceMock.Verify(x => x.RevokeAllAsync(userId, IpAddress, It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Publish(It.IsAny<PasswordChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidToken_ReturnsError()
        {
            var command = new ResetPasswordCommand("abdelrahman@example.com", "wrong-token", "NewPass123!");
            _authServiceMock
                .Setup(x => x.ResetPasswordAsync(command.Email, "wrong-token", command.NewPassword))
                .ReturnsAsync(Error.BadRequest("auth.invalid_reset_token"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
    }
}
