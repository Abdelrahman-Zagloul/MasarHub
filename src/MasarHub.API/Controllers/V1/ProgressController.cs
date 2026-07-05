using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Features.Progress.Commands.CompleteLesson;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Progress")]
    [Route("api/v{version:apiVersion}/progress")]
    [Authorize]
    public sealed class ProgressController : ApiControllerBase
    {
        private readonly ISender _sender;
        public ProgressController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }

        [HttpPost("lessons/complete")]
        [EndpointSummary("Complete a lesson")]
        [EndpointDescription("Marks a lesson as completed for the logged-in user and schedules progress recalculation.")]
        public async Task<IActionResult> CompleteLesson(CompleteLessonRequest request)
        {
            var result = await _sender.Send(new CompleteLessonCommand(GetUserId(), request.CourseId, request.LessonId));

            return await ToNoContentResultAsync(result);
        }
    }
}
