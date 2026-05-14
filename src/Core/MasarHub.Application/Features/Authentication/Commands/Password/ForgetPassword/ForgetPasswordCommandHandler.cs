using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.Password.ForgetPassword.Events;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Password.ForgetPassword
{
    public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;
        public ForgetPasswordCommandHandler(IAuthService authService, IMediator mediator)
        {
            _authService = authService;
            _mediator = mediator;
        }

        public async Task<Result> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.ForgetPasswordAsync(request.Email);

            if (result.IsSuccess)
                await _mediator.Publish(new PasswordResetRequestedEvent(result.Value));

            return Result.Success();
        }
    }
}
