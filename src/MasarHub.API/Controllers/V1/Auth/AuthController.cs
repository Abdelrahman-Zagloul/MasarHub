using Asp.Versioning;
using MasarHub.Application.Abstractions.Localization;
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
