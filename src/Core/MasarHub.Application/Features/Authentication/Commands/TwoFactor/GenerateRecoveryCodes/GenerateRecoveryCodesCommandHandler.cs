using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.GenerateRecoveryCodes
{
    public sealed class GenerateRecoveryCodesCommandHandler : IRequestHandler<GenerateRecoveryCodesCommand, Result<IEnumerable<string>>>
    {
        private readonly ITwoFactorService _twoFactorService;

        public GenerateRecoveryCodesCommandHandler(ITwoFactorService twoFactorService)
        {
            _twoFactorService = twoFactorService;
        }

        public async Task<Result<IEnumerable<string>>> Handle(GenerateRecoveryCodesCommand request, CancellationToken cancellationToken)
        {
            return await _twoFactorService.GenerateRecoveryCodesAsync(request.UserId);
        }
    }
}
