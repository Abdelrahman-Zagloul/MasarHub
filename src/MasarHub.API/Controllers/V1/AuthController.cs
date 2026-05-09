using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Features.Authentication.Commands.ConfirmEmail;
using MasarHub.Application.Features.Authentication.Commands.RegisterInstructor;
using MasarHub.Application.Features.Authentication.Commands.RegisterStudent;
using MediatR;
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

            return Ok(new
            {
                Message = await _localizationService.GetAsync(result.SuccessCode!)
            });
        }

        [HttpPost("instructor/register")]
        public async Task<IActionResult> RegisterInstructor(RegisterInstructorCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return Ok(new
            {
                Message = await _localizationService.GetAsync(result.SuccessCode!)
            });
        }

        [HttpPost("email/confirm")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest reuest)
        {
            var result = await _mediator.Send(new ConfirmEmailCommand(reuest.Email, reuest.Token, IpAddress));
            if (result.IsFailure)
                return await HandleError(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenResult.RefreshToken, result.Value.RefreshTokenResult.ExpiresAt);
            return Ok(new
            {
                Message = await _localizationService.GetAsync("auth.email_confirmed"),
                result.Value.AccessTokenResponse
            });
        }



        private void AddRefreshTokenToCookie(string refreshToken, DateTimeOffset expires)
        {
            Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expires
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

