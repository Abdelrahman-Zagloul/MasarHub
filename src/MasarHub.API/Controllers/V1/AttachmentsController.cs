using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.API.Extensions.Mappers;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Attachments.Commands.AddAttachment;
using MasarHub.Application.Features.Attachments.Commands.DeleteAttachment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Attachments")]
    [Route("api/v{version:apiVersion}/lessons/{lessonId:guid}/attachment")]
    public sealed class AttachmentsController : ApiBaseController
    {
        private readonly ISender _sender;

        public AttachmentsController(ILocalizationService localizationService, ISender sender)
            : base(localizationService)
        {
            _sender = sender;
        }

        [HttpPost()]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> AddLessonAttachment(Guid lessonId, IFormFile file)
        {
            var command = new AddAttachmentCommand(lessonId, GetUserId(), file.ToResource());
            var result = await _sender.Send(command);

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetAttachmentById), new { result.Value.LessonId, result.Value.AttachmentId }, result.Value);
        }


        [HttpDelete("{attachmentId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> DeleteAttachment(Guid lessonId, Guid attachmentId)
        {
            var result = await _sender.Send(new DeleteAttachmentCommand(lessonId, attachmentId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }


        [HttpGet("{attachmentId:guid}")]
        public IActionResult GetAttachmentById(Guid lessonId, Guid attachmentId)
        {
            return Ok();
        }
    }
}
