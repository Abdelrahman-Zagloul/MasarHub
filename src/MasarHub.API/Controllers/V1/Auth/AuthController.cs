using MasarHub.API.Extensions;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin;
using MasarHub.Application.Features.Authentication.Commands.Account.Login;
using MasarHub.Application.Features.Authentication.Commands.Account.Logout;
using MasarHub.Application.Features.Authentication.Commands.Account.RegisterInstructor;
using MasarHub.Application.Features.Authentication.Commands.Account.RegisterStudent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;


namespace MasarHub.API.Controllers.V1.Auth
{
    [Tags("Authentication")]
    [EnableRateLimiting(RateLimitingPolicies.Sensitive)]
    public class AuthController : AuthControllerBase
    {
        public AuthController(ILocalizationService localizationService, ISender sender)
              : base(localizationService, sender) { }

        [HttpPost("student/register")]
        [EndpointSummary("Register a new student account")]
        [EndpointDescription("Creates a new student account with the provided registration details and returns a success message upon completion.")]
        public async Task<IActionResult> RegisterStudent(RegisterStudentCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage(result.SuccessCode!);
        }

        [HttpPost("instructor/register")]
        [EndpointSummary("Register a new instructor account")]
        [EndpointDescription("Creates a new instructor account with the provided registration details and returns a success message upon completion.")]
        public async Task<IActionResult> RegisterInstructor(RegisterInstructorCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return await SuccessMessage(result.SuccessCode!);
        }

        [HttpPost("login")]
        [EndpointSummary("Authenticate user credentials")]
        [EndpointDescription("Authenticates the user with email and password. Returns JWT tokens on success, or triggers 2FA if enabled.")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _sender.Send(command);
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
        [EndpointSummary("Login with external provider")]
        [EndpointDescription("Authenticates the user using an external authentication provider (e.g., Google, Facebook) and returns JWT tokens.")]
        public async Task<IActionResult> ExternalLogin(ExternalLoginCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenResult);
            return Ok(result.Value.AccessTokenResponse);
        }

        [Authorize]
        [HttpPost("logout")]
        [EndpointSummary("Logout current session")]
        [EndpointDescription("Revokes the current refresh token and clears the refresh token cookie to end the user session.")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _sender.Send(new LogoutCommand(GetUserId(), IpAddress));
            RemoveRefreshTokenFromCookie();
            return await SuccessMessage("auth.logout_success");
        }
    }
}
