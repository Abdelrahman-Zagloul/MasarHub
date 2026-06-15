using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Account.RegisterStudent;
using MasarHub.Application.Features.Authentication.Commands.Account.RegisterStudent.Events;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;
using MediatR;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Account.RegisterStudent
{
    [Trait("UnitTests.Feature.Auth", "RegisterStudent")]
    public sealed class RegisterStudentCommandHandlerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RegisterStudentCommandHandler _sut;
        private readonly RegisterStudentCommand _validCommand;

        public RegisterStudentCommandHandlerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _mediatorMock = new Mock<IMediator>();
            _sut = new RegisterStudentCommandHandler(_authServiceMock.Object, _mediatorMock.Object);
            _validCommand = new RegisterStudentCommand("Abdelrahman", "abdelrahman@example.com", "01001234567", Gender.Male, "StrongPass123!");
        }

        [Fact]
        public async Task Handle_ValidRegistration_ReturnsSuccessAndPublishesEvent()
        {
            _authServiceMock
                .Setup(x => x.RegisterUserAsync(
                    _validCommand.FullName, _validCommand.Email, _validCommand.Password,
                    _validCommand.PhoneNumber, _validCommand.Gender, UserRole.Student,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RegisterUserResult>.Success(new RegisterUserResult("verify-token", Guid.NewGuid())));

            var result = await _sut.Handle(_validCommand, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.Is<StudentRegisteredEvent>(e =>
                e.Email == "abdelrahman@example.com"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_EmailAlreadyExists_ReturnsError()
        {
            _authServiceMock
                .Setup(x => x.RegisterUserAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<Gender>(), UserRole.Student,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.BadRequest("auth.email_already_exists"));

            var result = await _sut.Handle(_validCommand, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _mediatorMock.Verify(x => x.Publish(It.IsAny<StudentRegisteredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
