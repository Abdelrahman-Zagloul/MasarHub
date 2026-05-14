using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1.Auth
{
    [ApiVersion(1.0)]
    [Tags("Authentication")]
    [Route("api/auth")]
    public abstract class AuthBaseController : ApiBaseController
    {
        protected readonly IMediator _mediator;
        protected string? IpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();

        protected const string RefreshTokenCookieName = "refreshToken";

        protected AuthBaseController(ILocalizationService localizationService, IMediator mediator)
            : base(localizationService)
        {
            _mediator = mediator;
        }

        protected void AddRefreshTokenToCookie(RefreshTokenResult refreshTokenResult)
        {
            Response.Cookies.Append(RefreshTokenCookieName, refreshTokenResult.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = refreshTokenResult.ExpiresAt
            });
        }
        protected void RemoveRefreshTokenFromCookie()
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