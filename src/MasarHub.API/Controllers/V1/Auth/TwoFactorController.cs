using MasarHub.API.Extensions;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.GenerateRecoveryCodes;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.SendTwoFactorCode;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.SetupAuthenticator;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyAuthenticator;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyRecoveryCode;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyTwoFactorCode;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MasarHub.API.Controllers.V1.Auth
{
    [Tags("Authentication")]
    [EnableRateLimiting(RateLimitingPolicies.Sensitive)]
    public sealed class TwoFactorController : AuthBaseController
    {
        public TwoFactorController(ILocalizationService localizationService, ISender sender)
            : base(localizationService, sender) { }

        [Authorize]
        [HttpPost("2fa/enable")]
        [EndpointSummary("Enable two-factor authentication")]
        [EndpointDescription("Enables 2FA for the authenticated user after setting up an authenticator app.")]
        public async Task<IActionResult> EnableTwoFactorAuth(EnableTwoFactorCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);
            return await SuccessMessage("auth.2fa_enabled");
        }

        [Authorize]
        [HttpPost("2fa/disable")]
        [EndpointSummary("Disable two-factor authentication")]
        [EndpointDescription("Disables 2FA for the authenticated user.")]
        public async Task<IActionResult> Disable()
        {
            var result = await _sender.Send(new DisableTwoFactorCommand(GetUserId()));

            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.2fa_disabled");
        }

        [HttpPost("2fa/send-code")]
        [EndpointSummary("Send 2FA verification code")]
        [EndpointDescription("Sends a two-factor authentication code to the user's preferred channel (email or SMS) during login.")]
        [EnableRateLimiting(RateLimitingPolicies.Otp)]
        public async Task<IActionResult> SendCode(SendTwoFactorCodeCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.2fa_code_sent");
        }

        [HttpPost("2fa/verify")]
        [EndpointSummary("Verify 2FA code and complete login")]
        [EndpointDescription("Verifies the 2FA code sent to the user during login. Returns JWT tokens upon successful verification.")]
        [EnableRateLimiting(RateLimitingPolicies.Strict)]
        public async Task<IActionResult> VerifyCode(VerifyTwoFactorCodeCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenResult);
            return Ok(result.Value.AccessTokenResponse);
        }


        [Authorize]
        [HttpPost("2fa/authenticator/setup")]
        [EndpointSummary("Setup authenticator app")]
        [EndpointDescription("Generates a shared key and QR code URI for setting up an authenticator app (e.g., Google Authenticator).")]
        [EnableRateLimiting(RateLimitingPolicies.Otp)]
        public async Task<IActionResult> SetupAuthenticator()
        {
            var result = await _sender.Send(new SetupAuthenticatorCommand(GetUserId()));
            if (result.IsFailure)
                return await HandleError(result);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpPost("2fa/authenticator/verify")]
        [EndpointSummary("Verify authenticator setup")]
        [EndpointDescription("Verifies the authenticator app setup by validating a code generated from the app, then enables 2FA.")]
        [EnableRateLimiting(RateLimitingPolicies.Strict)]
        public async Task<IActionResult> VerifyAuthenticator(VerifyAuthenticatorCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.2fa_enabled");
        }

        [Authorize]
        [HttpPost("2fa/recovery-codes/generate")]
        [EndpointSummary("Generate 2FA recovery codes")]
        [EndpointDescription("Generates a new set of recovery codes for the authenticated user to access their account if they lose their 2FA device.")]
        [EnableRateLimiting(RateLimitingPolicies.Strict)]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var result = await _sender.Send(new GenerateRecoveryCodesCommand(GetUserId()));
            if (result.IsFailure)
                return await HandleError(result);

            return Ok(new
            {
                message = await _localizationService.GetAsync("auth.recovery_codes_generated"),
                codes = result.Value
            });
        }

        [HttpPost("2fa/recovery-codes/verify")]
        [EndpointSummary("Verify recovery code")]
        [EndpointDescription("Verifies a recovery code during login when the user cannot access their 2FA device. Returns JWT tokens on success.")]
        [EnableRateLimiting(RateLimitingPolicies.Strict)]
        public async Task<IActionResult> VerifyRecoveryCodes(VerifyRecoveryCodeCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenResult);
            return Ok(result.Value.AccessTokenResponse);
        }
    }
}
