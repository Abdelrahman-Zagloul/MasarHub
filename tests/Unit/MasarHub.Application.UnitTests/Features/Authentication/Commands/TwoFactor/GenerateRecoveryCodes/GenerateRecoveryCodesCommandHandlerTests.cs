using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.GenerateRecoveryCodes;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.TwoFactor.GenerateRecoveryCodes
{
    [Trait("UnitTests.Feature.Auth", "GenerateRecoveryCodes")]
    public sealed class GenerateRecoveryCodesCommandHandlerTests
    {
        private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
        private readonly GenerateRecoveryCodesCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();

        public GenerateRecoveryCodesCommandHandlerTests()
        {
            _twoFactorServiceMock = new Mock<ITwoFactorService>();
            _sut = new GenerateRecoveryCodesCommandHandler(_twoFactorServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsCodes()
        {
            var command = new GenerateRecoveryCodesCommand(UserId);
            var codes = new List<string> { "code1", "code2", "code3", "code4" };
            _twoFactorServiceMock
                .Setup(x => x.GenerateRecoveryCodesAsync(UserId))
                .ReturnsAsync(Result<IEnumerable<string>>.Success(codes));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(4);
        }

        [Fact]
        public async Task Handle_TwoFactorNotEnabled_ReturnsError()
        {
            var command = new GenerateRecoveryCodesCommand(UserId);
            _twoFactorServiceMock
                .Setup(x => x.GenerateRecoveryCodesAsync(UserId))
                .ReturnsAsync(Error.BadRequest("2fa.not_enabled"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
    }
}
