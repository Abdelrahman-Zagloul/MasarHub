using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor.Events;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyAuthenticator;
using MasarHub.Domain.Modules.Profiles;
using MediatR;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.TwoFactor.VerifyAuthenticator
{
    [Trait("UnitTests.Feature.Auth", "VerifyAuthenticator")]
    public sealed class VerifyAuthenticatorCommandHandlerTests
    {
        private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly VerifyAuthenticatorCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();

        public VerifyAuthenticatorCommandHandlerTests()
        {
            _twoFactorServiceMock = new Mock<ITwoFactorService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _mediatorMock = new Mock<IMediator>();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(UserId);
            _sut = new VerifyAuthenticatorCommandHandler(_twoFactorServiceMock.Object, _currentUserServiceMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCode_ReturnsSuccessAndPublishesEvent()
        {
            var command = new VerifyAuthenticatorCommand("123456");
            var enableResult = new EnableTwoFactorResult(UserId, "Abdelrahman", "abdelrahman@example.com", TwoFactorProvider.Authenticator);
            _twoFactorServiceMock
                .Setup(x => x.VerifyAuthenticatorSetupAsync(UserId, command.Code))
                .ReturnsAsync(Result<EnableTwoFactorResult>.Success(enableResult));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.IsAny<TwoFactorEnabledEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCode_ThrowsInvalidOperationException()
        {
            var command = new VerifyAuthenticatorCommand("wrong-code");
            _twoFactorServiceMock
                .Setup(x => x.VerifyAuthenticatorSetupAsync(UserId, command.Code))
                .ReturnsAsync(Error.BadRequest("2fa.invalid_code"));

            var act = async () => await _sut.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
