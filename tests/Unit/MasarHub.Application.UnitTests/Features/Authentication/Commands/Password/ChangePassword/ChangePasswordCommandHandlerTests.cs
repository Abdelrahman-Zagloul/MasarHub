using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword;
using MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword.Events;
using MediatR;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Password.ChangePassword
{
    [Trait("UnitTests", "Feature.Auth.ChangePassword.Handler")]
    public sealed class ChangePasswordCommandHandlerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly ChangePasswordCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();
        private const string IpAddress = "10.0.0.1";

        public ChangePasswordCommandHandlerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _mediatorMock = new Mock<IMediator>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _currentUserServiceMock.Setup(x => x.IpAddress).Returns(IpAddress);
            _sut = new ChangePasswordCommandHandler(_authServiceMock.Object, _mediatorMock.Object, _refreshTokenServiceMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidPasswordChange_ReturnsSuccessAndPublishesEvent()
        {
            var command = new ChangePasswordCommand(UserId, "OldPass123!", "NewPass456!");
            var passwordResult = new PasswordChangedResult(UserId, "Abdelrahman", "abdelrahman@example.com");
            _authServiceMock
                .Setup(x => x.ChangePasswordAsync(UserId, command.CurrentPassword, command.NewPassword))
                .ReturnsAsync(Result<PasswordChangedResult>.Success(passwordResult));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _refreshTokenServiceMock.Verify(x => x.RevokeAllAsync(UserId, IpAddress, It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Publish(It.IsAny<PasswordChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_IncorrectCurrentPassword_ReturnsError()
        {
            var command = new ChangePasswordCommand(UserId, "WrongOldPass!", "NewPass456!");
            _authServiceMock
                .Setup(x => x.ChangePasswordAsync(UserId, command.CurrentPassword, command.NewPassword))
                .ReturnsAsync(Error.BadRequest("auth.invalid_current_password"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
    }
}
