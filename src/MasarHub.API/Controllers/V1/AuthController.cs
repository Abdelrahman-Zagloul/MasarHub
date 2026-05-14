using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Features.Authentication.Commands.ChangePassword;
using MasarHub.Application.Features.Authentication.Commands.ConfirmEmail;
using MasarHub.Application.Features.Authentication.Commands.ForgetPassword;
using MasarHub.Application.Features.Authentication.Commands.Logout;
using MasarHub.Application.Features.Authentication.Commands.RefreshToken;
using MasarHub.Application.Features.Authentication.Commands.RegisterInstructor;
using MasarHub.Application.Features.Authentication.Commands.RegisterStudent;
using MasarHub.Application.Features.Authentication.Commands.ResendConfirmEmail;
using MasarHub.Application.Features.Authentication.Commands.ResetPassword;
using MasarHub.Application.Features.Authentication.Commands.RevokeToken;
using MasarHub.Application.Features.Authentication.Commands.VerifyPassword;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    public sealed class AuthController : ApiBaseController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IMediator _mediator;
        private string? IpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();
        private const string RefreshTokenCookieName = "refreshToken";
        public AuthController(ILocalizationService localizationService, IMediator mediator) : base(localizationService)
        {
            _localizationService = localizationService;
            _mediator = mediator;
        }

        [HttpPost("student/register")]
        public async Task<IActionResult> RegisterStudent(RegisterStudentCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage(result.SuccessCode!);
        }

        [HttpPost("instructor/register")]
        public async Task<IActionResult> RegisterInstructor(RegisterInstructorCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage(result.SuccessCode!);
        }

        [HttpPost("email/confirm")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
        {
            var result = await _mediator.Send(new ConfirmEmailCommand(request.Email, request.Token, IpAddress));
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

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _mediator.Send(new LogoutCommand(GetUserId(), IpAddress));
            RemoveRefreshTokenFromCookie();
            return await SuccessMessage("auth.logout_success");
        }

        [Authorize]
        [HttpPost("token/revoke")]
        public async Task<IActionResult> RevokeTokenAsync()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            var result = await _mediator.Send(new RevokeTokenCommand(refreshToken, IpAddress));
            if (result.IsFailure)
                return await HandleError(result);

            RemoveRefreshTokenFromCookie();
            return await SuccessMessage("auth.token_revoked");
        }

        [HttpPost("token/refresh")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken, IpAddress));
            if (result.IsFailure)
                return await HandleError(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenResult);
            return Ok(result.Value.AccessTokenResponse);
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

        private void AddRefreshTokenToCookie(RefreshTokenResult refreshTokenResult)
        {
            Response.Cookies.Append(RefreshTokenCookieName, refreshTokenResult.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = refreshTokenResult.ExpiresAt
            });
        }
        private void RemoveRefreshTokenFromCookie()
        {
            Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
        }
    }
}

