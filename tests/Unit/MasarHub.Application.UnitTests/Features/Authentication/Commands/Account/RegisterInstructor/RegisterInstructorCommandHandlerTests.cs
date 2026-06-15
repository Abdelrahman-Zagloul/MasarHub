using FluentAssertions;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Account.RegisterInstructor;
using MasarHub.Application.Features.Authentication.Commands.Account.RegisterInstructor.Events;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;
using MediatR;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Account.RegisterInstructor
{
    [Trait("UnitTests.Feature.Auth", "RegisterInstructor")]
    public sealed class RegisterInstructorCommandHandlerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<InstructorProfile>> _repositoryMock;
        private readonly RegisterInstructorCommandHandler _sut;

        public RegisterInstructorCommandHandlerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _mediatorMock = new Mock<IMediator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _repositoryMock = new Mock<IRepository<InstructorProfile>>();
            _sut = new RegisterInstructorCommandHandler(_authServiceMock.Object, _mediatorMock.Object, _unitOfWorkMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRegistration_ReturnsSuccessAndSavesProfile()
        {
            var userId = Guid.NewGuid();
            var command = new RegisterInstructorCommand(
                "Abdelrahman", "abdelrahman@example.com", "01001234567",
                Gender.Male, "StrongPass123!", "Software Engineer",
                "Experienced developer", "Tech Corp",
                [new SocialLinkRequest("GitHub", "https://github.com/abdelrahman")]);

            _authServiceMock
                .Setup(x => x.RegisterUserAsync(
                    command.FullName, command.Email, command.Password,
                    command.PhoneNumber, command.Gender, UserRole.Instructor,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RegisterUserResult>.Success(new RegisterUserResult("verify-token", userId)));
            _mediatorMock
                .Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<InstructorProfile>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Publish(It.IsAny<InstructorRegisteredEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_RegistrationFails_ReturnsError()
        {
            var command = new RegisterInstructorCommand(
                "Abdelrahman", "abdelrahman@example.com", "01001234567",
                Gender.Male, "StrongPass123!", "Software Engineer", null, null, []);

            _authServiceMock
                .Setup(x => x.RegisterUserAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<Gender>(), UserRole.Instructor,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.BadRequest("auth.email_already_exists"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_SocialLinkCreationFails_DeletesCreatedUserAndReturnsError()
        {
            var userId = Guid.NewGuid();
            var command = new RegisterInstructorCommand(
                "Abdelrahman", "abdelrahman@example.com", "01001234567",
                Gender.Male, "StrongPass123!", "Headline", "Bio", "Company",
                [new SocialLinkRequest("InvalidPlatform", "not-a-valid-url")]);

            _authServiceMock
                .Setup(x => x.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<Gender>(), UserRole.Instructor, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RegisterUserResult>.Success(new RegisterUserResult("verify-token", userId)));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _authServiceMock.Verify(x => x.DeleteUserAsync(userId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ProfileCreationFails_DeletesCreatedUserAndReturnsError()
        {
            var userId = Guid.NewGuid();
            var command = new RegisterInstructorCommand(
                "Abdelrahman", "abdelrahman@example.com", "01001234567",
                Gender.Male, "StrongPass123!", string.Empty, "Bio", "Company", []);

            _authServiceMock
                .Setup(x => x.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<Gender>(), UserRole.Instructor, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RegisterUserResult>.Success(new RegisterUserResult("verify-token", userId)));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _authServiceMock.Verify(x => x.DeleteUserAsync(userId), Times.Once);
            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<InstructorProfile>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AddSocialLinkToProfileFails_DeletesCreatedUserAndReturnsError()
        {
            var userId = Guid.NewGuid();
            var command = new RegisterInstructorCommand(
                "Abdelrahman", "abdelrahman@example.com", "01001234567",
                Gender.Male, "StrongPass123!", "Headline", "Bio", "Company",
                [
                    new SocialLinkRequest("GitHub", "https://github.com/abdelrahman"),
                    new SocialLinkRequest("LinkedIn", "https://github.com/abdelrahman")
                ]);

            _authServiceMock
                .Setup(x => x.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<Gender>(), UserRole.Instructor, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RegisterUserResult>.Success(new RegisterUserResult("verify-token", userId)));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _authServiceMock.Verify(x => x.DeleteUserAsync(userId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
