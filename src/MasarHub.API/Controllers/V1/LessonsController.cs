using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.API.Extensions.Mappers;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Lessons.Commands.AddVideoLesson;
using MasarHub.Application.Features.Lessons.Commands.CreateArticleLesson;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Lessons")]
    [Route("api/courses/{courseId:guid}/modules/{moduleId:guid}/lessons")]
    public class LessonsController : ApiBaseController
    {
        private readonly ISender _sender;
        public LessonsController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }

        [HttpPost("article")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> CreateArticleLesson(Guid courseId, Guid moduleId, CreateArticleLessonRequest request)
        {
            var result = await _sender.Send(new CreateArticleLessonCommand(
                courseId, moduleId, GetUserId(), request.IsPreviewable, request.Title, request.Content, request.Description));

            return await ToCreatedActionResultAsync(result, nameof(GetLessonById), new { courseId, moduleId, result.Value.Id });
        }

        [HttpPost("video")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> AddVideoLesson(Guid courseId, Guid moduleId, [FromForm] AddVideoLessonRequest request)
        {
            var command = new AddVideoLessonCommand(
                courseId, moduleId, GetUserId(), request.IsPreviewable,
                request.Title, request.Description, request.VideoFile.ToResource());

            var result = await _sender.Send(command);

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetLessonById), new { courseId, moduleId, result.Value.Id }, result.Value);
        }


        [HttpGet]
        public IActionResult GetLessonById(Guid courseId, Guid moduleId, Guid lessonId)
        {
            return Ok();
        }
    }
    public sealed record AddVideoLessonRequest(bool IsPreviewable, string Title, string? Description, IFormFile VideoFile);
}
