using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.API.Extensions.Mappers;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Lessons.Commands.AddArticleLesson;
using MasarHub.Application.Features.Lessons.Commands.AddVideoLesson;
using MasarHub.Application.Features.Lessons.Commands.ArchiveLesson;
using MasarHub.Application.Features.Lessons.Commands.CreateArticleLesson;
using MasarHub.Application.Features.Lessons.Commands.UpdateLesson;
using MasarHub.Application.Features.Lessons.Commands.DeleteLesson;
using MasarHub.Application.Features.Lessons.Commands.DisableLessonPreview;
using MasarHub.Application.Features.Lessons.Commands.EnableLessonPreview;
using MasarHub.Application.Features.Lessons.Commands.ReorderLessons;
using MasarHub.Application.Features.Lessons.Commands.UnarchiveLesson;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Lessons")]
    [Route("api/v{version:apiVersion}/modules/{moduleId:guid}/lessons")]
    public class LessonsController : ApiBaseController
    {
        private readonly ISender _sender;
        public LessonsController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }

        [HttpPost("article")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> AddArticleLesson(Guid moduleId, AddArticleLessonRequest request)
        {
            var result = await _sender.Send(new AddArticleLessonCommand(
                 moduleId, GetUserId(), request.IsPreviewable, request.Title, request.Content, request.Description));

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetLessonById), new { moduleId, result.Value.Id }, result.Value);
        }

        [HttpPost("video")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> AddVideoLesson(Guid moduleId, [FromForm] AddVideoLessonRequest request)
        {
            var command = new AddVideoLessonCommand(
                moduleId, GetUserId(), request.IsPreviewable,
                request.Title, request.Description, request.VideoFile.ToResource());

            var result = await _sender.Send(command);

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetLessonById), new { moduleId, result.Value.Id }, result.Value);
        }


        [HttpPut("{lessonId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> UpdateLesson(Guid moduleId, Guid lessonId, UpdateLessonRequest request)
        {
            var result = await _sender.Send(new UpdateLessonCommand(moduleId, lessonId, GetUserId(), request.Title, request.Description));
            return await ToNoContentResultAsync(result);
        }

        [HttpDelete("{lessonId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> DeleteLesson(Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new DeleteLessonCommand(moduleId, lessonId, GetUserId()));

            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("{lessonId:guid}/archive")]
        public async Task<IActionResult> ArchiveLesson(Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new ArchiveLessonCommand(moduleId, lessonId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("{lessonId:guid}/unarchive")]
        public async Task<IActionResult> UnarchiveLesson(Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new UnarchiveLessonCommand(moduleId, lessonId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("{lessonId:guid}/preview/enable")]
        public async Task<IActionResult> EnableLessonPreview(Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new EnableLessonPreviewCommand(moduleId, lessonId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("{lessonId:guid}/preview/disable")]
        public async Task<IActionResult> DisableLessonPreview(Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new DisableLessonPreviewCommand(moduleId, lessonId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("reorder")]
        public async Task<IActionResult> ReorderLesson(Guid moduleId, ReorderLessonsRequest request)
        {
            var result = await _sender.Send(new ReorderLessonsCommand(moduleId, GetUserId(), request.OrderedLessonIds));
            return await ToNoContentResultAsync(result);
        }

        [HttpGet]
        public IActionResult GetLessonById(Guid moduleId, Guid lessonId)
        {
            return Ok();
        }
    }
    public sealed record AddVideoLessonRequest(bool IsPreviewable, string Title, string? Description, IFormFile VideoFile);
}
