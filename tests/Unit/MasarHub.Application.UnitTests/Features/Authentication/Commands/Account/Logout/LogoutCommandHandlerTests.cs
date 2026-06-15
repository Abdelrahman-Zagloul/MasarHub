using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Features.Authentication.Commands.Account.Logout;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Account.Logout
{
    [Trait("UnitTests.Feature.Auth", "Logout")]
    public sealed class LogoutCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly LogoutCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();
        private const string IpAddress = "192.168.1.1";

        public LogoutCommandHandlerTests()
        {
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _sut = new LogoutCommandHandler(_refreshTokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidLogout_ReturnsSuccess()
        {
            var command = new LogoutCommand(UserId, IpAddress);
            _refreshTokenServiceMock
                .Setup(x => x.RevokeAllAsync(UserId, IpAddress, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }
    }
}
