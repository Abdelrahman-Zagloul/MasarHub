using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Features.Authentication.Commands.Token.RefreshToken;
using MasarHub.Application.Features.Authentication.Commands.Token.RevokeToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1.Auth
{
    [Tags("Authentication")]
    public sealed class TokenController : AuthControllerBase
    {
        public TokenController(ILocalizationService localizationService, ISender sender)
            : base(localizationService, sender) { }

        [Authorize]
        [HttpPost("token/revoke")]
        [EndpointSummary("Revoke refresh token")]
        [EndpointDescription("Revokes the current refresh token retrieved from the cookie, terminating the session.")]
        public async Task<IActionResult> RevokeTokenAsync()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            var result = await _sender.Send(new RevokeTokenCommand(refreshToken, IpAddress));
            if (result.IsFailure)
                return await HandleError(result);

            RemoveRefreshTokenFromCookie();
            return await SuccessMessage("auth.token_revoked");
        }

        [HttpPost("token/refresh")]
        [EndpointSummary("Refresh JWT tokens")]
        [EndpointDescription("Uses the refresh token stored in the cookie to issue a new JWT access token and a new refresh token.")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            var result = await _sender.Send(new RefreshTokenCommand(refreshToken, IpAddress));
            if (result.IsFailure)
                return await HandleError(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenResult);
            return Ok(result.Value.AccessTokenResponse);
        }

    }
}
