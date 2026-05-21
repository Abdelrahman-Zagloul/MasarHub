using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.SendCode;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.SetupAuthenticator;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyAuthenticator;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyCode;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1.Auth
{
    [Tags("Authentication")]
    public sealed class TwoFactorController : AuthBaseController
    {
        public TwoFactorController(ILocalizationService localizationService, IMediator mediator)
            : base(localizationService, mediator) { }

        [Authorize]
        [HttpPost("2fa/enable")]
        public async Task<IActionResult> EnableTwoFactorAuth(EnableTwoFactorCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);
            return await SuccessMessage("auth.2fa_enabled");
        }

        [Authorize]
        [HttpPost("2fa/disable")]
        public async Task<IActionResult> Disable()
        {
            var result = await _mediator.Send(new DisableTwoFactorCommand(GetUserId()));

            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.2fa_disabled");
        }

        [HttpPost("2fa/send-code")]
        public async Task<IActionResult> SendCode(SendTwoFactorCodeCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.2fa_code_sent");
        }

        [HttpPost("2fa/verify")]
        public async Task<IActionResult> VerifyCode(VerifyTwoFactorCodeCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenResult);
            return Ok(result.Value.AccessTokenResponse);
        }


        [Authorize]
        [HttpPost("2fa/authenticator/setup")]
        public async Task<IActionResult> SetupAuthenticator()
        {
            var result = await _mediator.Send(new SetupAuthenticatorCommand(GetUserId()));
            if (result.IsFailure)
                return await HandleError(result);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpPost("afa/authenticator/verify")]
        public async Task<IActionResult> VerifyAuthenticator(VerifyAuthenticatorCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.2fa_enabled");
        }
    }
}
