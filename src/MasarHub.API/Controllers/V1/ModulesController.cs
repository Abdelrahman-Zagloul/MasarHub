using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Features.Modules.Commands.CreateModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Modules")]
    [Route("api/courses/{courseId:guid}/modules")]
    public sealed class ModulesController : ApiBaseController
    {
        private readonly IMediator _mediator;
        public ModulesController(ILocalizationService localizationService, IMediator mediator) : base(localizationService)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid courseId, CreateModuleRequest request)
        {
            var result = await _mediator.Send(new CreateModuleCommand(courseId, GetUserId(), request.Title, request.Description));
            if (result.IsFailure)
                return await HandleError(result);

            return CreatedAtAction(nameof(GetById), new { courseId, moduleId = result.Value.Id }, result.Value);
        }

        [HttpGet("{moduleId:guid}")]
        public async Task<IActionResult> GetById(Guid courseId, Guid moduleId)
        {
            return Ok();
        }
    }
}
