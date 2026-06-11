using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Features.Lessons.Commands.DeleteLessonAttachment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Attachments")]
    [Route("api/lessons/{lessonId:guid}/attachment")]
    public sealed class AttachmentsController : ApiBaseController
    {
        private readonly ISender _sender;

        public AttachmentsController(ILocalizationService localizationService, ISender sender)
            : base(localizationService)
        {
            _sender = sender;
        }

        [HttpDelete("{attachmentId:guid}")]
        public async Task<IActionResult> DeleteAttachment(Guid lessonId, Guid attachmentId)
        {
            var result = await _sender.Send(new DeleteLessonAttachmentCommand(lessonId, attachmentId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }
    }
}
