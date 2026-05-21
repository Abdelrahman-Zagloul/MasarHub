using Asp.Versioning;
using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin;
using MasarHub.Application.Features.Authentication.Commands.Account.Login;
using MasarHub.Application.Features.Authentication.Commands.Account.Logout;
using MasarHub.Application.Features.Authentication.Commands.Account.RegisterInstructor;
using MasarHub.Application.Features.Authentication.Commands.Account.RegisterStudent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1.Auth
{
    [ApiVersion(1.0)]
    [Tags("Authentication")]
    [Route("api/auth")]
    public class AuthController : AuthBaseController
    {
        public AuthController(ILocalizationService localizationService, IMediator mediator)
              : base(localizationService, mediator)
        {
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            if (result.Value.RequiresTwoFactor)
            {
                return Ok(new
                {
                    Message = await _localizationService.GetAsync("auth.2fa_required"),
                    result.Value.RequiresTwoFactor,
                    result.Value.ChallengeId,
                    result.Value.Provider,
                });
            }

            AddRefreshTokenToCookie(result.Value.Tokens!.RefreshTokenResult);
            return Ok(new
            {
                Message = await _localizationService.GetAsync("auth.login_success"),
                result.Value.Tokens.AccessTokenResponse
            });
        }

        [HttpPost("external/login")]
        public async Task<IActionResult> ExternalLogin(ExternalLoginCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenResult);
            return Ok(result.Value.AccessTokenResponse);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _mediator.Send(new LogoutCommand(GetUserId(), IpAddress));
            RemoveRefreshTokenFromCookie();
            return await SuccessMessage("auth.logout_success");
        }
    }
}
