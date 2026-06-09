using MasarHub.API.Extensions;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword;
using MasarHub.Application.Features.Authentication.Commands.Password.ForgetPassword;
using MasarHub.Application.Features.Authentication.Commands.Password.ResetPassword;
using MasarHub.Application.Features.Authentication.Commands.Password.VerifyPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MasarHub.API.Controllers.V1.Auth
{
    [Tags("Authentication")]
    public sealed class PasswordController : AuthBaseController
    {
        public PasswordController(ILocalizationService localizationService, ISender sender)
            : base(localizationService, sender) { }

        [Authorize]
        [HttpPost("password/change")]
        [EnableRateLimiting(RateLimitingPolicies.Sensitive)]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var result = await _sender.Send(new ChangePasswordCommand(GetUserId(), request.CurrentPassword, request.NewPassword));
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.password_changed");
        }

        [HttpPost("password/forget")]
        [EnableRateLimiting(RateLimitingPolicies.Otp)]
        public async Task<IActionResult> ForgetPasswordAsync(ForgetPasswordCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.forget_password");
        }

        [HttpPost("password/reset")]
        [EnableRateLimiting(RateLimitingPolicies.Strict)]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage("auth.password_reset");
        }

        [Authorize]
        [HttpPost("password/verify")]
        [EnableRateLimiting(RateLimitingPolicies.Sensitive)]
        public async Task<IActionResult> VerifyPassword(VerifyPasswordCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return Ok(new
            {
                verified = true
            });
        }
    }
}
