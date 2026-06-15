using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor.Events;
using MasarHub.Domain.Modules.Profiles;
using MediatR;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.TwoFactor.EnableTwoFactor
{
    [Trait("UnitTests.Feature.Auth", "EnableTwoFactor")]
    public sealed class EnableTwoFactorCommandHandlerTests
    {
        private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly EnableTwoFactorCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();

        public EnableTwoFactorCommandHandlerTests()
        {
            _twoFactorServiceMock = new Mock<ITwoFactorService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _mediatorMock = new Mock<IMediator>();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(UserId);
            _sut = new EnableTwoFactorCommandHandler(_twoFactorServiceMock.Object, _currentUserServiceMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_ValidProvider_ReturnsSuccessAndPublishesEvent()
        {
            var command = new EnableTwoFactorCommand(TwoFactorProvider.Authenticator);
            var result = new EnableTwoFactorResult(UserId, "Abdelrahman", "abdelrahman@example.com", TwoFactorProvider.Authenticator);
            _twoFactorServiceMock
                .Setup(x => x.EnableAsync(UserId, TwoFactorProvider.Authenticator))
                .ReturnsAsync(Result<EnableTwoFactorResult>.Success(result));

            var response = await _sut.Handle(command, CancellationToken.None);

            response.IsSuccess.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.IsAny<TwoFactorEnabledEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_EnableFails_ReturnsError()
        {
            var command = new EnableTwoFactorCommand(TwoFactorProvider.Authenticator);
            _twoFactorServiceMock
                .Setup(x => x.EnableAsync(UserId, TwoFactorProvider.Authenticator))
                .ReturnsAsync(Error.BadRequest("2fa.already_enabled"));

            var response = await _sut.Handle(command, CancellationToken.None);

            response.IsFailure.Should().BeTrue();
        }
    }
}
