using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Exams.Commands.CreateExam;
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

        [HttpGet("{id:guid}")]
        [EndpointSummary("Get exam by ID (Not Implemented Now)")]
        [EndpointDescription("Retrieves a specific exam by its ID. Currently a Not Implemented endpoint returning OK.")]
        public IActionResult GetExamById(Guid id)
        {
            return Ok();
        }
    }
}
