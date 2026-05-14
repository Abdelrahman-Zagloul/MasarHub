using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor.Events;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor
{
    public sealed class EnableTwoFactorCommandHandler : IRequestHandler<EnableTwoFactorCommand, Result>
    {
        private readonly ITwoFactorService _twoFactorService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        public EnableTwoFactorCommandHandler(ITwoFactorService woFactorService, ICurrentUserService currentUserService, IMediator mediator)
        {
            _twoFactorService = woFactorService;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<Result> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var result = await _twoFactorService.EnableAsync(_currentUserService.UserId, request.Provider);
            if (result.IsFailure)
                return result;

            await _mediator.Publish(new TwoFactorEnabledEvent(result.Value));
            return Result.Success();
        }
    }
}
