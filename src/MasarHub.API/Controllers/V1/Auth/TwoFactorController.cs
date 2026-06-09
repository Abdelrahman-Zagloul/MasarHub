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
        public async Task<IActionResult> EnableTwoFactorAuth(EnableTwoFactorCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);
            return await SuccessMessage("auth.2fa_enabled");
        }

        [Authorize]
        [HttpPost("2fa/disable")]
        public async Task<IActionResult> Disable()
        {
            var result = await _sender.Send(new DisableTwoFactorCommand(GetUserId()));

            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.2fa_disabled");
        }

        [HttpPost("2fa/send-code")]
        [EnableRateLimiting(RateLimitingPolicies.Otp)]
        public async Task<IActionResult> SendCode(SendTwoFactorCodeCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.2fa_code_sent");
        }

        [HttpPost("2fa/verify")]
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
