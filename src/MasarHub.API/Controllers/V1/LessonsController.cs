using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.API.Extensions.Mappers;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Lessons.Commands.AddArticleLesson;
using MasarHub.Application.Features.Lessons.Commands.AddLessonAttachment;
using MasarHub.Application.Features.Lessons.Commands.AddVideoLesson;
using MasarHub.Application.Features.Lessons.Commands.ArchiveLesson;
using MasarHub.Application.Features.Lessons.Commands.CreateArticleLesson;
using MasarHub.Application.Features.Lessons.Commands.DeleteLesson;
using MasarHub.Application.Features.Lessons.Commands.UnarchiveLesson;
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
        public async Task<IActionResult> AddArticleLesson(Guid courseId, Guid moduleId, AddArticleLessonRequest request)
        {
            var result = await _sender.Send(new AddArticleLessonCommand(
                courseId, moduleId, GetUserId(), request.IsPreviewable, request.Title, request.Content, request.Description));

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetLessonById), new { courseId, moduleId, result.Value.Id }, result.Value);
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

        [HttpPost("{lessonId:guid}/attachment")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> AddLessonAttachment(Guid courseId, Guid moduleId, Guid lessonId, IFormFile file)
        {
            var command = new AddLessonAttachmentCommand(courseId, moduleId, lessonId, GetUserId(), file.ToResource());
            var result = await _sender.Send(command);

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetLessonById), new { courseId, moduleId, result.Value.LessonId }, result.Value);
        }

        [HttpDelete("{lessonId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> DeleteLesson(Guid courseId, Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new DeleteLessonCommand(courseId, moduleId, lessonId, GetUserId()));

            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("{lessonId:guid}/archive")]
        public async Task<IActionResult> ArchiveLesson(Guid courseId, Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new ArchiveLessonCommand(courseId, moduleId, lessonId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("{lessonId:guid}/unarchive")]
        public async Task<IActionResult> UnarchiveLesson(Guid courseId, Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new UnarchiveLessonCommand(courseId, moduleId, lessonId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }


        [HttpGet]
        public IActionResult GetLessonById(Guid courseId, Guid moduleId, Guid lessonId)
        {
            return Ok();
        }
    }
    public sealed record AddVideoLessonRequest(bool IsPreviewable, string Title, string? Description, IFormFile VideoFile);
}
