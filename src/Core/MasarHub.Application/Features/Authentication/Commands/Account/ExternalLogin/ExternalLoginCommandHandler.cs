using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.Email.ConfirmEmail.Events;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin
{
    public sealed class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, Result<AccessWithRefreshTokenResult>>
    {
        private readonly IMediator _mediator;
        private readonly IExternalAuthService _externalAuthService;
        private readonly IEnumerable<IExternalAuthProvider> _externalAuthProviders;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ICurrentUserService _currentUserService;
        public ExternalLoginCommandHandler(
            IMediator mediator,
            IExternalAuthService externalAuthService,
            IEnumerable<IExternalAuthProvider> externalAuthProviders,
            ITokenService tokenService,
            IRefreshTokenService refreshTokenService,
            ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _externalAuthService = externalAuthService;
            _externalAuthProviders = externalAuthProviders;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<AccessWithRefreshTokenResult>> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
        {
            var provider = _externalAuthProviders.First(x => x.Provider == request.Provider);
            var verificationResult = await provider.VerifyAsync(request.Token, cancellationToken);
            if (verificationResult.IsFailure)
                return verificationResult.Errors[0];

            var loginResult = await _externalAuthService.LoginAsync(verificationResult.Value);
            if (loginResult.IsFailure)
                return loginResult.Errors[0];

            var accessToken = await _tokenService.GenerateTokenAsync(loginResult.Value.User);
            var refreshTokenResult = await _refreshTokenService.CreateAsync(loginResult.Value.User, _currentUserService.IpAddress, cancellationToken);
            if (refreshTokenResult.IsFailure)
                return refreshTokenResult.Errors[0];

            if (loginResult.Value.IsNew)
                await _mediator.Publish(new EmailConfirmedEvent(loginResult.Value.User));

            return new AccessWithRefreshTokenResult(accessToken, refreshTokenResult.Value);
        }
    }
}
