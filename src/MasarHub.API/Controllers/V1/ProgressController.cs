using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Progress.Commands.CompleteLesson;
using MasarHub.Application.Features.Progress.Queries.GetCourseProgress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Progress")]
    [Route("api/v{version:apiVersion}/progress")]
    public sealed class ProgressController : ApiControllerBase
    {
        private readonly ISender _sender;
        public ProgressController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }

        [Authorize(Roles = Roles.Student)]
        [HttpGet("courses/{courseId:guid}")]
        [EndpointSummary("Get course progress")]
        [EndpointDescription("Returns the logged-in user's progress for a specific course.")]
        public async Task<IActionResult> GetCourseProgress(Guid courseId)
        {
            var result = await _sender.Send(new GetCourseProgressQuery(GetUserId(), courseId));
            return await ToOkResultAsync(result);
        }


        [HttpPost("lessons/complete")]
        [Authorize(Roles = Roles.Student)]
        [EndpointSummary("Complete a lesson")]
        [EndpointDescription("Marks a lesson as completed for the logged-in user and schedules progress recalculation.")]
        public async Task<IActionResult> CompleteLesson(CompleteLessonRequest request)
        {
            var result = await _sender.Send(new CompleteLessonCommand(GetUserId(), request.CourseId, request.LessonId));
            if (result.IsFailure)
                return await HandleError(result);

            return result.SuccessCode != null
                ? await SuccessMessage(result.SuccessCode)
                : NoContent();
        }
    }
}
