using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor.Events;
using MediatR;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.TwoFactor.DisableTwoFactor
{
    [Trait("UnitTests", "Feature.Auth.DisableTwoFactor.Handler")]
    public sealed class DisableTwoFactorCommandHandlerTests
    {
        private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DisableTwoFactorCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();

        public DisableTwoFactorCommandHandlerTests()
        {
            _twoFactorServiceMock = new Mock<ITwoFactorService>();
            _mediatorMock = new Mock<IMediator>();
            _sut = new DisableTwoFactorCommandHandler(_twoFactorServiceMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessAndPublishesEvent()
        {
            var command = new DisableTwoFactorCommand(UserId);
            var result = new DisableTwoFactorResult(UserId, "Abdelrahman", "abdelrahman@example.com");
            _twoFactorServiceMock
                .Setup(x => x.DisableAsync(UserId))
                .ReturnsAsync(Result<DisableTwoFactorResult>.Success(result));

            var response = await _sut.Handle(command, CancellationToken.None);

            response.IsSuccess.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.IsAny<TwoFactorDisabledEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AlreadyDisabled_ReturnsError()
        {
            var command = new DisableTwoFactorCommand(UserId);
            _twoFactorServiceMock
                .Setup(x => x.DisableAsync(UserId))
                .ReturnsAsync(Error.BadRequest("2fa.already_disabled"));

            var response = await _sut.Handle(command, CancellationToken.None);

            response.IsFailure.Should().BeTrue();
        }
    }
}
