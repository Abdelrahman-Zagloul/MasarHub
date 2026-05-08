using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.RegisterStudent.Events;
using MasarHub.Domain.Modules.Profiles;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.RegisterStudent
{
    public sealed class RegisterStudentCommandHandler : IRequestHandler<RegisterStudentCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;
        public RegisterStudentCommandHandler(IAuthService authService, IMediator mediator)
        {
            _authService = authService;
            _mediator = mediator;
        }

        public async Task<Result> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
        {
            var registerUserResult = await _authService.RegisterUserAsync(
                request.FullName,
                request.Email,
                request.Password,
                request.PhoneNumber,
                request.Gender,
                UserRole.Student,
                cancellationToken);

            if (registerUserResult.IsFailure)
                return registerUserResult;

            await _mediator.Publish(new StudentRegisteredEvent(request.FullName, request.Email, registerUserResult.Value.EmailVerificationToken), cancellationToken);

            return Result.Success("auth.registration.success");
        }
    }
}
