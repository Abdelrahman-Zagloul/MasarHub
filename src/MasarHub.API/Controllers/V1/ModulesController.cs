using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Modules.Commands.CreateModule;
using MasarHub.Application.Features.Modules.Commands.UpdateModule;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = Roles.Instructor)]
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

        [HttpPut("{moduleId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> Update(Guid courseId, Guid moduleId, UpdateModuleRequest request)
        {
            var result = await _mediator.Send(new UpdateModuleCommand(courseId, moduleId, GetUserId(), request.Title, request.Description));
            if (result.IsFailure)
                return await HandleError(result);

            return NoContent();
        }
    }
}
