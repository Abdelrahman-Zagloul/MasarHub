using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Token.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AccessWithRefreshTokenResult>>
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenService _accessTokenService;
        private readonly IAuthService _authService;
        public RefreshTokenCommandHandler(IRefreshTokenService refreshTokenService, ITokenService accessTokenService, IAuthService authService)
        {
            _refreshTokenService = refreshTokenService;
            _accessTokenService = accessTokenService;
            _authService = authService;
        }

        public async Task<Result<AccessWithRefreshTokenResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return Error.Unauthorized("auth.refresh_token_missing");

            var refreshTokenResult = await _refreshTokenService.RotateAsync(request.RefreshToken, request.IpAddress, cancellationToken);
            if (refreshTokenResult.IsFailure)
                return refreshTokenResult.Errors[0];

            var userResult = await _authService.GetUserAsync(refreshTokenResult.Value.UserId);
            if (userResult.IsFailure)
                return userResult.Errors[0];

            var accessTokenResponse = await _accessTokenService.GenerateTokenAsync(userResult.Value);
            return new AccessWithRefreshTokenResult(accessTokenResponse, refreshTokenResult.Value);
        }
    }
}
