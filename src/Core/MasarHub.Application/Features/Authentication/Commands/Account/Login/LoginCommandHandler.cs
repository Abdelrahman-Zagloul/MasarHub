using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResult>>
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITwoFactorChallengeStore _twoFactorChallengeStore;
        public LoginCommandHandler(IAuthService authService, ICurrentUserService currentUserService, ITokenService tokenService, IRefreshTokenService refreshTokenService, ITwoFactorChallengeStore twoFactorChallengeStore)
        {
            _authService = authService;
            _currentUserService = currentUserService;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _twoFactorChallengeStore = twoFactorChallengeStore;
        }

        public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var authenticateUserResult = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
            if (authenticateUserResult.IsFailure)
                return authenticateUserResult.Errors[0];

            if (authenticateUserResult.Value.RequiresTwoFactor)
            {

                var challengeId = await _twoFactorChallengeStore.CreateAsync(
                        authenticateUserResult.Value.TokenUser.Id,
                        authenticateUserResult.Value.Provider!.Value,
                        cancellationToken);

                return LoginResult.TwoFactorRequired(challengeId, authenticateUserResult.Value.Provider!.Value);
            }

            var accessToken = await _tokenService.GenerateTokenAsync(authenticateUserResult.Value.TokenUser);
            var refreshToken = await _refreshTokenService.CreateAsync(
                authenticateUserResult.Value.TokenUser,
                _currentUserService.IpAddress,
                cancellationToken);

            if (refreshToken.IsFailure)
                return refreshToken.Errors[0];

            return LoginResult.Success(new AccessWithRefreshTokenResult(accessToken, refreshToken.Value));
        }
    }
}