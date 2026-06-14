using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.SendTwoFactorCode;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.TwoFactor.SendTwoFactorCode
{
    [Trait("UnitTests", "Feature.Auth.SendTwoFactorCode.Handler")]
    public sealed class SendTwoFactorCodeCommandHandlerTests
    {
        private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
        private readonly SendTwoFactorCodeCommandHandler _sut;
        private static readonly Guid ChallengeId = Guid.NewGuid();

        public SendTwoFactorCodeCommandHandlerTests()
        {
            _twoFactorServiceMock = new Mock<ITwoFactorService>();
            _sut = new SendTwoFactorCodeCommandHandler(_twoFactorServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccess()
        {
            var command = new SendTwoFactorCodeCommand(ChallengeId);
            _twoFactorServiceMock
                .Setup(x => x.SendCodeAsync(ChallengeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_TwoFactorNotEnabled_ReturnsError()
        {
            var command = new SendTwoFactorCodeCommand(ChallengeId);
            _twoFactorServiceMock
                .Setup(x => x.SendCodeAsync(ChallengeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.BadRequest("2fa.not_enabled"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
    }
}
