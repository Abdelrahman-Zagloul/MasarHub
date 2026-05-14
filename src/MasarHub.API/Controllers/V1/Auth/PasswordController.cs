using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword;
using MasarHub.Application.Features.Authentication.Commands.Password.ForgetPassword;
using MasarHub.Application.Features.Authentication.Commands.Password.ResetPassword;
using MasarHub.Application.Features.Authentication.Commands.Password.VerifyPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1.Auth
{
    [Tags("Authentication")]
    public sealed class PasswordController : AuthBaseController
    {
        public PasswordController(ILocalizationService localizationService, IMediator mediator)
            : base(localizationService, mediator)
        {
        }
        [Authorize]
        [HttpPost("password/change")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var result = await _mediator.Send(new ChangePasswordCommand(GetUserId(), request.CurrentPassword, request.NewPassword));
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.password_changed");
        }

        [HttpPost("password/forget")]
        public async Task<IActionResult> ForgetPasswordAsync(ForgetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.forget_password");
        }

        [HttpPost("password/reset")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.password_reset");
        }

        [Authorize]
        [HttpPost("password/verify")]
        public async Task<IActionResult> VerifyPassword(VerifyPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return Ok(new
            {
                verified = true
            });
        }
    }
}
