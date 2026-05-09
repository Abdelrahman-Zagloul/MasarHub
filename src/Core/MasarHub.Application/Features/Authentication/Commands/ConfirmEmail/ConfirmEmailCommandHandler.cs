using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.ConfirmEmail.Events;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.ConfirmEmail
{
    public sealed class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<AccessWithRefreshTokenResult>>
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IMediator _mediator;
        public ConfirmEmailCommandHandler(IAuthService authService, ITokenService tokenService, IRefreshTokenService refreshTokenService, IMediator mediator)
        {
            _authService = authService;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _mediator = mediator;
        }

        public async Task<Result<AccessWithRefreshTokenResult>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var token = Uri.UnescapeDataString(request.Token);
            var confirmResult = await _authService.ConfirmEmailAsync(request.Email, token, cancellationToken);
            if (confirmResult.IsFailure)
                return confirmResult.Errors[0];


            var accessToken = await _tokenService.GenerateTokenAsync(confirmResult.Value.User);
            var refreshTokenResult = await _refreshTokenService.CreateAsync(confirmResult.Value.User, request.IpAddress, cancellationToken);

            if (refreshTokenResult.IsFailure)
                return refreshTokenResult.Errors[0];

            await _mediator.Publish(new EmailConfirmedEvent(confirmResult.Value.User, confirmResult.Value.FullName));

            return new AccessWithRefreshTokenResult(accessToken, refreshTokenResult.Value);
        }
    }
}
