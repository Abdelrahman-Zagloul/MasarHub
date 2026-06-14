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
    public sealed class EmailController : AuthControllerBase
    {
        public EmailController(ILocalizationService localizationService, ISender sender)
            : base(localizationService, sender) { }

        [HttpPost("email/confirm")]
        [EndpointSummary("Confirm email address")]
        [EndpointDescription("Confirms the user's email address using a confirmation token sent via email. Returns JWT tokens upon successful confirmation.")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand command)
        {
            var result = await _sender.Send(command);
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
        [EndpointSummary("Resend email confirmation")]
        [EndpointDescription("Resends the email confirmation message to the user's registered email address.")]
        public async Task<IActionResult> ResendConfirmEmailAsync(ResendConfirmEmailCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.confirmation_email_sent");
        }
    }
}
