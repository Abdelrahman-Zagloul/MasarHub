using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Email.ResendConfirmEmail;
using MasarHub.Application.Features.Authentication.Commands.Email.ResendConfirmEmail.Events;
using MediatR;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Email.ResendConfirmEmail
{
    [Trait("UnitTests.Feature.Auth", "ResendConfirmEmail")]
    public sealed class ResendConfirmEmailCommandHandlerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ResendConfirmEmailCommandHandler _sut;

        public ResendConfirmEmailCommandHandlerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _mediatorMock = new Mock<IMediator>();
            _sut = new ResendConfirmEmailCommandHandler(_authServiceMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_UnconfirmedEmail_ReturnsSuccessAndPublishesEvent()
        {
            var command = new ResendConfirmEmailCommand("abdelrahman@example.com");
            var tokenResult = new ConfirmEmailTokenResult("Abdelrahman", "abdelrahman@example.com", "new-token");
            _authServiceMock
                .Setup(x => x.GenerateEmailTokenAsync(command.Email))
                .ReturnsAsync(Result<ConfirmEmailTokenResult>.Success(tokenResult));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.IsAny<ConfirmEmailTokenCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AlreadyConfirmedEmail_ReturnsError()
        {
            var command = new ResendConfirmEmailCommand("abdelrahman@example.com");
            _authServiceMock
                .Setup(x => x.GenerateEmailTokenAsync(command.Email))
                .ReturnsAsync(Error.BadRequest("auth.email.already_confirmed"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.IsAny<ConfirmEmailTokenCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UnknownEmail_ReturnsSuccessSilently()
        {
            var command = new ResendConfirmEmailCommand("unknown@example.com");
            _authServiceMock
                .Setup(x => x.GenerateEmailTokenAsync(command.Email))
                .ReturnsAsync(Result<ConfirmEmailTokenResult>.Failure(new List<Error> { Error.NotFound("user.not_found") }));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }
    }
}
