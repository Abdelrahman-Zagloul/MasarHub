using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.ChangePassword.Events;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.ResetPassword
{
    public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ICurrentUserService _currentUserService;
        public ResetPasswordCommandHandler(IAuthService authService, IMediator mediator, IRefreshTokenService refreshTokenService, ICurrentUserService currentUserService)
        {
            _authService = authService;
            _mediator = mediator;
            _refreshTokenService = refreshTokenService;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var decodedToken = Uri.UnescapeDataString(request.Token);

            var result = await _authService.ResetPasswordAsync(request.Email, decodedToken, request.NewPassword);
            if (result.IsFailure)
                return result;

            await _refreshTokenService.RevokeAllAsync(result.Value.UserId, _currentUserService.IpAddress, cancellationToken);
            await _mediator.Publish(new PasswordChangedEvent(result.Value));

            return Result.Success();
        }
    }
}
