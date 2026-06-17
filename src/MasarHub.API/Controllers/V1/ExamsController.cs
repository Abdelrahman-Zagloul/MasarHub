using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Exams.Commands.CreateExam;
using MasarHub.Application.Features.Exams.Commands.DeleteExam;
using MasarHub.Application.Features.Exams.Commands.ToggleExamPublished;
using MasarHub.Application.Features.Exams.Commands.UpdateExam;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Exams")]
    [Route("api/v{version:apiVersion}/exams")]
    public sealed class ExamsController : ApiControllerBase
    {
        private readonly ISender _sender;

        public ExamsController(ILocalizationService localizationService, ISender sender)
            : base(localizationService)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Create an exam")]
        [EndpointDescription("Creates a new exam for a course. Optionally associates it with a specific module. Instructor only.")]
        public async Task<IActionResult> Create(Guid courseId, CreateExamRequest request)
        {
            var command = new CreateExamCommand(courseId, GetUserId(), request.Title, request.PassingScorePercentage, request.MaxAttempts, request.ModuleId, request.Description, request.DurationMinutes);
            var result = await _sender.Send(command);

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetExamById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Update exam details")]
        [EndpointDescription("Updates the title, description, passing score, and/or duration of an exam. Instructor only.")]
        public async Task<IActionResult> UpdateExam(Guid id, UpdateExamRequest request)
        {
            var command = new UpdateExamCommand(id, GetUserId(), request.Title, request.Description, request.MaxAttempts, request.PassingScorePercentage, request.DurationMinutes);
            var result = await _sender.Send(command);
            return await ToNoContentResultAsync(result);
        }

        [HttpPatch("{id:guid}/published")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Toggle exam published state")]
        [EndpointDescription("Toggles the published state of an exam (published/unpublished). Instructor only.")]
        public async Task<IActionResult> ToggleExamPublished(Guid id, ToggleExamPublishedRequest request)
        {
            var command = new ToggleExamPublishedCommand(id, GetUserId(), request.IsPublished);
            var result = await _sender.Send(command);
            return await ToNoContentResultAsync(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Delete an exam")]
        [EndpointDescription("Soft-deletes an exam. Instructor only.")]
        public async Task<IActionResult> DeleteExam(Guid id)
        {
            var result = await _sender.Send(new DeleteExamCommand(id, GetUserId()));
            return await ToNoContentResultAsync(result);
        }

        [HttpGet("{id:guid}")]
        [EndpointSummary("Get exam by ID (Not Implemented Now)")]
        [EndpointDescription("Retrieves a specific exam by its ID. Currently a Not Implemented endpoint returning OK.")]
        public IActionResult GetExamById(Guid id)
        {
            return Ok();
        }
    }
}
