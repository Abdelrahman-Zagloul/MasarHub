using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.SetupAuthenticator
{
    public sealed class SetupAuthenticatorCommandHandler : IRequestHandler<SetupAuthenticatorCommand, Result<SetupAuthenticatorResult>>
    {
        private readonly ITwoFactorService _twoFactorService;

        public SetupAuthenticatorCommandHandler(ITwoFactorService twoFactorService)
        {
            _twoFactorService = twoFactorService;
        }

        public async Task<Result<SetupAuthenticatorResult>> Handle(SetupAuthenticatorCommand request, CancellationToken cancellationToken)
        {
            return await _twoFactorService.SetupAuthenticatorAsync(request.UserId, cancellationToken);
        }
    }
}
