using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Localization;
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
        public AuthController(ILocalizationService localizationService, IMediator mediator) : base(localizationService)
        {
            _localizationService = localizationService;
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterStudent(RegisterStudentCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return Ok(result);
        }

    }
}
