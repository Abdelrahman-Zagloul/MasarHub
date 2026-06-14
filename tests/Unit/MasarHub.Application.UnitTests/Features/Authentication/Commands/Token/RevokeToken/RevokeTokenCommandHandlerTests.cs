using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Token.RevokeToken;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Token.RevokeToken
{
    [Trait("UnitTests", "Feature.Auth.RevokeToken.Handler")]
    public sealed class RevokeTokenCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly RevokeTokenCommandHandler _sut;
        private const string IpAddress = "10.0.0.1";

        public RevokeTokenCommandHandlerTests()
        {
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _sut = new RevokeTokenCommandHandler(_refreshTokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidToken_ReturnsSuccess()
        {
            var command = new RevokeTokenCommand("valid-refresh-token", IpAddress);
            _refreshTokenServiceMock
                .Setup(x => x.RevokeAsync("valid-refresh-token", IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_MissingToken_ReturnsUnauthorized()
        {
            var command = new RevokeTokenCommand(null, IpAddress);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_InvalidToken_ReturnsError()
        {
            var command = new RevokeTokenCommand("invalid-token", IpAddress);
            _refreshTokenServiceMock
                .Setup(x => x.RevokeAsync("invalid-token", IpAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.NotFound("auth.refresh_token_not_found"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
    }
}
