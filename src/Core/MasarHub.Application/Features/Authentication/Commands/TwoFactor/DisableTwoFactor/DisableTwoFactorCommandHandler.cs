using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor.Events;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor
{
    public sealed class DisableTwoFactorCommandHandler : IRequestHandler<DisableTwoFactorCommand, Result>
    {
        private readonly ITwoFactorService _twoFactorService;
        private readonly IMediator _mediator;

        public DisableTwoFactorCommandHandler(ITwoFactorService twoFactorService, IMediator mediator)
        {
            _twoFactorService = twoFactorService;
            _mediator = mediator;
        }

        public async Task<Result> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var result = await _twoFactorService.DisableAsync(request.UserId);

            if (result.IsFailure)
                return result;

            await _mediator.Publish(new TwoFactorDisabledEvent(result.Value), cancellationToken);

            return Result.Success("auth.2fa_disabled");
        }
    }
}
