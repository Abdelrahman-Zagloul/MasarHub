using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Features.Authentication.Commands.Token.RefreshToken;
using MasarHub.Application.Features.Authentication.Commands.Token.RevokeToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1.Auth
{
    [Tags("Authentication")]
    public sealed class TokenController : AuthBaseController
    {
        public TokenController(ILocalizationService localizationService, IMediator mediator)
            : base(localizationService, mediator)
        {
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

    }
}
