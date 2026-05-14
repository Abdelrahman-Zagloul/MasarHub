using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Token.RevokeToken
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Result>
    {
        private readonly IRefreshTokenService _refreshTokenService;

        public RevokeTokenCommandHandler(IRefreshTokenService refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }

        public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return Error.Unauthorized("auth.refresh_token_missing");

            return await _refreshTokenService
                .RevokeAsync(request.RefreshToken, request.IpAddress, cancellationToken);
        }
    }
}
