using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.SendCode
{
    public sealed class SendTwoFactorCodeCommandHandler : IRequestHandler<SendTwoFactorCodeCommand, Result>
    {

        private readonly ITwoFactorService _twoFactorService;

        public SendTwoFactorCodeCommandHandler(ITwoFactorService twoFactorService)
        {
            _twoFactorService = twoFactorService;
        }

        public async Task<Result> Handle(SendTwoFactorCodeCommand request, CancellationToken cancellationToken)
        {
            return await _twoFactorService.SendCodeAsync(request.ChallengeId, cancellationToken);
        }
    }
}
