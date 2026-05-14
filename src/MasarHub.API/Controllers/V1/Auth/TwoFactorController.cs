using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor;
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
    }
}
