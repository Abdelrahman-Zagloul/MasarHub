using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Modules.Commands.CreateModule;
using MasarHub.Application.Features.Modules.Commands.DeleteModule;
using MasarHub.Application.Features.Modules.Commands.ReorderModules;
using MasarHub.Application.Features.Modules.Commands.UpdateModule;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Modules")]
    [Route("api/v{version:apiVersion}/courses/{courseId:guid}/modules")]
    public sealed class ModulesController : ApiBaseController
    {
        private readonly ISender _sender;
        public ModulesController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }


        [HttpPost]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> Create(Guid courseId, CreateModuleRequest request)
        {
            var result = await _sender.Send(new CreateModuleCommand(courseId, GetUserId(), request.Title, request.Description));

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetModuleById), new { courseId, moduleId = result.Value.Id }, result.Value);
        }


        [HttpGet("{moduleId:guid}")]
        public async Task<IActionResult> GetModuleById(Guid courseId, Guid moduleId)
        {
            return Ok();
        }


        [HttpPut("{moduleId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> UpdateModule(Guid courseId, Guid moduleId, UpdateModuleRequest request)
        {
            var result = await _sender.Send(new UpdateModuleCommand(courseId, moduleId, GetUserId(), request.Title, request.Description));
            return await ToNoContentResultAsync(result);

        }


        [HttpDelete("{moduleId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> DeleteModule(Guid courseId, Guid moduleId)
        {
            var result = await _sender.Send(new DeleteModuleCommand(courseId, moduleId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }


        [HttpPatch("reorder")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> ReorderModules(Guid courseId, ReorderModulesRequest request)
        {
            var result = await _sender.Send(new ReorderModulesCommand(courseId, GetUserId(), request.OrderedModuleIds));
            return await ToNoContentResultAsync(result);
        }
    }
}
