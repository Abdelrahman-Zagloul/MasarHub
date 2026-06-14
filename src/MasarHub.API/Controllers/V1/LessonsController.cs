using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.API.Extensions.Mappers;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Lessons.Commands.AddArticleLesson;
using MasarHub.Application.Features.Lessons.Commands.AddVideoLesson;
using MasarHub.Application.Features.Lessons.Commands.ArchiveLesson;
using MasarHub.Application.Features.Lessons.Commands.CreateArticleLesson;
using MasarHub.Application.Features.Lessons.Commands.DeleteLesson;
using MasarHub.Application.Features.Lessons.Commands.DisableLessonPreview;
using MasarHub.Application.Features.Lessons.Commands.EnableLessonPreview;
using MasarHub.Application.Features.Lessons.Commands.ReorderLessons;
using MasarHub.Application.Features.Lessons.Commands.UnarchiveLesson;
using MasarHub.Application.Features.Lessons.Commands.UpdateLesson;
using MasarHub.Application.Features.Lessons.Commands.UpdateVideoThumbnail;
using MasarHub.Application.Features.Lessons.Queries.GetVideoUploadSignature;
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
        [EndpointSummary("Add an article lesson")]
        [EndpointDescription("Creates a new article-type lesson with title, content, and description within the specified module. Instructor only.")]
        public async Task<IActionResult> AddArticleLesson(Guid moduleId, AddArticleLessonRequest request)
        {
            var result = await _sender.Send(new AddArticleLessonCommand(
                 moduleId, GetUserId(), request.IsPreviewable, request.Title, request.Content, request.Description));

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetLessonById), new { moduleId, lessonId = result.Value.Id }, result.Value);
        }

        [HttpPost("video")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Add a video lesson")]
        [EndpointDescription("Creates a new video-type lesson using a previously uploaded file key within the specified module. Instructor only.")]
        public async Task<IActionResult> AddVideoLesson(Guid moduleId, AddVideoLessonRequest request)
        {
            var command = new AddVideoLessonCommand(moduleId, GetUserId(), request.IsPreviewable, request.Title, request.Description, request.FileKey);
            var result = await _sender.Send(command);

            return result.IsFailure
               ? await HandleError(result)
               : CreatedAtAction(nameof(GetLessonById), new { moduleId, lessonId = result.Value.Id }, result.Value);
        }

        [HttpGet("video-upload/signature")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Get video upload signature")]
        [EndpointDescription("Generates a signed upload URL for direct browser-to-cloud video upload. Returns the signature, timestamp, and public ID.")]
        public async Task<IActionResult> GetVideoUploadSignature(Guid moduleId)
        {
            var result = await _sender.Send(new GetVideoUploadSignatureQuery(moduleId, GetUserId()));
            return await ToOkResultAsync(result);
        }


        [HttpPut("{lessonId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Update lesson details")]
        [EndpointDescription("Updates the title and description of an existing lesson within the specified module. Instructor only.")]
        public async Task<IActionResult> UpdateLesson(Guid moduleId, Guid lessonId, UpdateLessonRequest request)
        {
            var result = await _sender.Send(new UpdateLessonCommand(moduleId, lessonId, GetUserId(), request.Title, request.Description));
            return await ToNoContentResultAsync(result);
        }

        [HttpDelete("{lessonId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Delete a lesson")]
        [EndpointDescription("Permanently deletes a lesson from the specified module. Instructor only.")]
        public async Task<IActionResult> DeleteLesson(Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new DeleteLessonCommand(moduleId, lessonId, GetUserId()));

            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("{lessonId:guid}/archive")]
        [EndpointSummary("Archive a lesson")]
        [EndpointDescription("Archives a lesson, hiding it from students while preserving its content. Instructor only.")]
        public async Task<IActionResult> ArchiveLesson(Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new ArchiveLessonCommand(moduleId, lessonId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("{lessonId:guid}/unarchive")]
        [EndpointSummary("Unarchive a lesson")]
        [EndpointDescription("Restores an archived lesson, making it visible to students again. Instructor only.")]
        public async Task<IActionResult> UnarchiveLesson(Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new UnarchiveLessonCommand(moduleId, lessonId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("{lessonId:guid}/preview/enable")]
        [EndpointSummary("Enable lesson preview")]
        [EndpointDescription("Allows non-enrolled students to preview this lesson without purchasing the course. Instructor only.")]
        public async Task<IActionResult> EnableLessonPreview(Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new EnableLessonPreviewCommand(moduleId, lessonId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("{lessonId:guid}/preview/disable")]
        [EndpointSummary("Disable lesson preview")]
        [EndpointDescription("Restricts lesson preview to only enrolled students. Instructor only.")]
        public async Task<IActionResult> DisableLessonPreview(Guid moduleId, Guid lessonId)
        {
            var result = await _sender.Send(new DisableLessonPreviewCommand(moduleId, lessonId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("{lessonId:guid}/thumbnail")]
        [EndpointSummary("Update video lesson thumbnail")]
        [EndpointDescription("Uploads a custom thumbnail image for a video lesson. The old thumbnail is automatically cleaned up. Instructor only.")]
        public async Task<IActionResult> UpdateVideoThumbnail(Guid moduleId, Guid lessonId, IFormFile file)
        {
            var result = await _sender.Send(new UpdateVideoThumbnailCommand(moduleId, lessonId, GetUserId(), file.ToResource()));
            return await ToNoContentResultAsync(result);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPatch("reorder")]
        [EndpointSummary("Reorder lessons")]
        [EndpointDescription("Changes the display order of lessons within a module by providing an ordered list of lesson IDs. Instructor only.")]
        public async Task<IActionResult> ReorderLesson(Guid moduleId, ReorderLessonsRequest request)
        {
            var result = await _sender.Send(new ReorderLessonsCommand(moduleId, GetUserId(), request.OrderedLessonIds));
            return await ToNoContentResultAsync(result);
        }

        [HttpGet("{lessonId:guid}")]
        [EndpointSummary("Get lesson by ID (stub)")]
        [EndpointDescription("Retrieves a specific lesson within a module. Currently a stub endpoint returning OK.")]
        public IActionResult GetLessonById(Guid moduleId, Guid lessonId)
        {
            return Ok();
        }
    }
}
