using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyRecoveryCode.Events;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyRecoveryCode
{

    public sealed class VerifyRecoveryCodeCommandHandler : IRequestHandler<VerifyRecoveryCodeCommand, Result<AccessWithRefreshTokenResult>>
    {
        private readonly ITwoFactorService _twoFactorService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        public VerifyRecoveryCodeCommandHandler(ITwoFactorService twoFactorService, ITokenService tokenService, IRefreshTokenService refreshTokenService, ICurrentUserService currentUserService, IMediator mediator)
        {
            _twoFactorService = twoFactorService;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<Result<AccessWithRefreshTokenResult>> Handle(VerifyRecoveryCodeCommand request, CancellationToken cancellationToken)
        {
            var verificationResult = await _twoFactorService.VerifyRecoveryCodeAsync(request.ChallengeId, request.Code);
            if (verificationResult.IsFailure)
                return verificationResult.Errors[0];

            var accessToken = await _tokenService.GenerateTokenAsync(verificationResult.Value);
            var refreshTokenResult = await _refreshTokenService.CreateAsync(verificationResult.Value, _currentUserService.IpAddress, cancellationToken);
            if (refreshTokenResult.IsFailure)
                return refreshTokenResult.Errors[0];

            await _mediator.Publish(new TwoFactorRecoveryCodeUsedEvent(verificationResult.Value));

            return new AccessWithRefreshTokenResult(accessToken, refreshTokenResult.Value);
        }
    }
}
