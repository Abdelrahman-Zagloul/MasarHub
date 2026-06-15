using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Password.VerifyPassword;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Password.VerifyPassword
{
    [Trait("UnitTests.Feature.Auth", "VerifyPassword")]
    public sealed class VerifyPasswordCommandHandlerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly VerifyPasswordCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();

        public VerifyPasswordCommandHandlerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(UserId);
            _sut = new VerifyPasswordCommandHandler(_authServiceMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidPassword_ReturnsSuccess()
        {
            var command = new VerifyPasswordCommand("CorrectPass123!");
            _authServiceMock
                .Setup(x => x.VerifyPasswordAsync(UserId, command.Password))
                .ReturnsAsync(Result.Success());

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_InvalidPassword_ReturnsError()
        {
            var command = new VerifyPasswordCommand("WrongPass!");
            _authServiceMock
                .Setup(x => x.VerifyPasswordAsync(UserId, command.Password))
                .ReturnsAsync(Error.BadRequest("auth.invalid_password"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
    }
}
