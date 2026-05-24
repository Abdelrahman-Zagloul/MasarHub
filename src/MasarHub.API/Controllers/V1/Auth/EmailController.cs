using MasarHub.API.Extensions;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Features.Authentication.Commands.Email.ConfirmEmail;
using MasarHub.Application.Features.Authentication.Commands.Email.ResendConfirmEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MasarHub.API.Controllers.V1.Auth
{
    [Tags("Authentication")]
    [EnableRateLimiting(RateLimitingPolicies.Otp)]
    public sealed class EmailController : AuthBaseController
    {
        public EmailController(ILocalizationService localizationService, IMediator mediator)
            : base(localizationService, mediator) { }

        [HttpPost("email/confirm")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenResult);
            return Ok(new
            {
                Message = await _localizationService.GetAsync("auth.email_confirmed"),
                result.Value.AccessTokenResponse
            });
        }

        [HttpPost("email/resend-confirmation")]
        public async Task<IActionResult> ResendConfirmEmailAsync(ResendConfirmEmailCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.confirmation_email_sent");
        }
    }
}
