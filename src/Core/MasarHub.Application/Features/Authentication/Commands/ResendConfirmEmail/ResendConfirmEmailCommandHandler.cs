using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.ResendConfirmEmail.Events;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.ResendConfirmEmail
{
    public sealed class ResendConfirmEmailCommandHandler : IRequestHandler<ResendConfirmEmailCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;
        public ResendConfirmEmailCommandHandler(IAuthService authService, IMediator mediator)
        {
            _authService = authService;
            _mediator = mediator;
        }

        public async Task<Result> Handle(ResendConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var emailTokenResult = await _authService.GenerateEmailTokenAsync(request.Email);
            if (emailTokenResult.IsFailure)
            {
                if (emailTokenResult.Errors.Any(x => x.Code == "auth.email.already_confirmed"))
                    return emailTokenResult;

                return Result.Success();
            }

            await _mediator.Publish(new ConfirmEmailTokenCreatedEvent(emailTokenResult.Value));

            return Result.Success();
        }
    }
}
