using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyTwoFactorCode
{
    public sealed class VerifyTwoFactorCodeCommandHandler : IRequestHandler<VerifyTwoFactorCodeCommand, Result<AccessWithRefreshTokenResult>>
    {
        private readonly ITwoFactorService _twoFactorService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;

        public VerifyTwoFactorCodeCommandHandler(ITwoFactorService twoFactorService, IRefreshTokenService refreshTokenService, ITokenService tokenService, ICurrentUserService currentUserService)
        {
            _twoFactorService = twoFactorService;
            _refreshTokenService = refreshTokenService;
            _tokenService = tokenService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<AccessWithRefreshTokenResult>> Handle(VerifyTwoFactorCodeCommand request, CancellationToken cancellationToken)
        {
            var verificationResult = await _twoFactorService
                .VerifyCodeAsync(request.ChallengeId, request.Code, cancellationToken);

            if (verificationResult.IsFailure)
                return verificationResult.Errors[0];

            var accessToken = await _tokenService.GenerateTokenAsync(verificationResult.Value);

            var refreshTokenResult = await _refreshTokenService
                .CreateAsync(verificationResult.Value, _currentUserService.IpAddress, cancellationToken);
            if (refreshTokenResult.IsFailure)
                return refreshTokenResult.Errors[0];

            return new AccessWithRefreshTokenResult(accessToken, refreshTokenResult.Value);
        }
    }
}
