using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1.Auth
{
    [ApiVersion(1.0)]
    [Tags("Authentication")]
    [Route("api/v{version:apiVersion}/auth")]
    public abstract class AuthControllerBase : ApiControllerBase
    {
        protected readonly ISender _sender;
        protected string? IpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();

        protected const string RefreshTokenCookieName = "refreshToken";

        protected AuthControllerBase(ILocalizationService localizationService, ISender sender)
            : base(localizationService)
        {
            _sender = sender;
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