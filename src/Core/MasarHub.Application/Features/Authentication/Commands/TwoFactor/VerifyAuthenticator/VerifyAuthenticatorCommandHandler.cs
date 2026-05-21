using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor.Events;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyAuthenticator
{
    public sealed class VerifyAuthenticatorCommandHandler : IRequestHandler<VerifyAuthenticatorCommand, Result>
    {
        private readonly ITwoFactorService _twoFactorService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public VerifyAuthenticatorCommandHandler(ITwoFactorService twoFactorService, ICurrentUserService currentUserService, IMediator mediator)
        {
            _twoFactorService = twoFactorService;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<Result> Handle(VerifyAuthenticatorCommand request, CancellationToken cancellationToken)
        {
            var result = await _twoFactorService.VerifyAuthenticatorSetupAsync(_currentUserService.UserId, request.Code);


            await _mediator.Publish(new TwoFactorEnabledEvent(result.Value));
            return Result.Success();
        }
    }
}
