using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Password.ForgetPassword;
using MasarHub.Application.Features.Authentication.Commands.Password.ForgetPassword.Events;
using MediatR;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Password.ForgetPassword
{
    [Trait("UnitTests.Feature.Auth", "ForgetPassword")]
    public sealed class ForgetPasswordCommandHandlerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ForgetPasswordCommandHandler _sut;

        public ForgetPasswordCommandHandlerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _mediatorMock = new Mock<IMediator>();
            _sut = new ForgetPasswordCommandHandler(_authServiceMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_ExistingEmail_ReturnsSuccessAndPublishesEvent()
        {
            var command = new ForgetPasswordCommand("abdelrahman@example.com");
            var resultData = new ForgetPasswordResult("Abdelrahman", "abdelrahman@example.com", "reset-token");
            _authServiceMock
                .Setup(x => x.ForgetPasswordAsync(command.Email))
                .ReturnsAsync(Result<ForgetPasswordResult>.Success(resultData));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.IsAny<PasswordResetRequestedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentEmail_StillReturnsSuccessSilently()
        {
            var command = new ForgetPasswordCommand("unknown@example.com");
            _authServiceMock
                .Setup(x => x.ForgetPasswordAsync(command.Email))
                .ReturnsAsync(Result<ForgetPasswordResult>.Failure(Error.NotFound("user.not_found")));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.IsAny<PasswordResetRequestedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
