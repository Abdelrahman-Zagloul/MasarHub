using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.ChangePassword.Events;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;
        private readonly IRefreshTokenService _refreshTokenService;
        public ChangePasswordCommandHandler(IAuthService authService, IMediator mediator, IRefreshTokenService refreshTokenService)
        {
            _authService = authService;
            _mediator = mediator;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.ChangePasswordAsync(
                request.UserId,
                request.CurrentPassword,
                request.NewPassword,
                cancellationToken);

            if (result.IsFailure)
                return result;

            await _refreshTokenService.RevokeAllAsync(request.UserId, null, cancellationToken);

            await _mediator.Publish(new PasswordChangedEvent(result.Value));

            return Result.Success();
        }

    }
}
