using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Announcements.Commands.CreateCourseAnnouncement;
using MasarHub.Application.Features.Announcements.Commands.PublishCourseAnnouncement;
using MasarHub.Application.Features.Announcements.Commands.ScheduleCourseAnnouncement;
using MasarHub.Application.Features.Announcements.Commands.SetAnnouncementPin;
using MasarHub.Application.Features.Announcements.Commands.UpdateCourseAnnouncement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Announcements")]
    [Route("api/v{version:apiVersion}/courses/{courseId:guid}/announcements")]
    public sealed class AnnouncementsController : ApiControllerBase
    {
        private readonly ISender _sender;
        public AnnouncementsController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }


        [HttpPost]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Create course announcement")]
        [EndpointDescription("Creates a new draft announcement for the course. Instructor only.")]
        public async Task<IActionResult> CreateCourseAnnouncement(Guid courseId, CreateCourseAnnouncementRequest request)
        {
            var command = new CreateCourseAnnouncementCommand(courseId, GetUserId(), request.Title, request.Content, request.Importance);

            var result = await _sender.Send(command);
            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetById), new { courseId, id = result.Value.Id }, result.Value);
        }

        [HttpPut("{announcementId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Update course announcement")]
        [EndpointDescription("Updates title, content, and importance of a draft announcement. Instructor only.")]
        public async Task<IActionResult> UpdateCourseAnnouncement(Guid courseId, Guid announcementId, UpdateCourseAnnouncementRequest request)
        {
            var command = new UpdateCourseAnnouncementCommand(courseId, announcementId, GetUserId(), request.Title, request.Content, request.Importance);
            var result = await _sender.Send(command);
            return await ToNoContentResultAsync(result);
        }

        [HttpPut("{announcementId:guid}/publish")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Publish course announcement")]
        [EndpointDescription("Publishes a draft announcement and notifies enrolled students. Instructor only.")]
        public async Task<IActionResult> PublishCourseAnnouncement(Guid courseId, Guid announcementId)
        {
            var command = new PublishCourseAnnouncementCommand(courseId, announcementId, GetUserId());
            var result = await _sender.Send(command);
            return await ToNoContentResultAsync(result);
        }

        [HttpPut("{announcementId:guid}/pin")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Set announcement pin")]
        [EndpointDescription("Pins or unpins a course announcement. Instructor only.")]
        public async Task<IActionResult> SetAnnouncementPin(Guid courseId, Guid announcementId, SetAnnouncementPinRequest request)
        {
            var command = new SetAnnouncementPinCommand(courseId, announcementId, GetUserId(), request.IsPinned);
            var result = await _sender.Send(command);
            return await ToNoContentResultAsync(result);
        }

        [HttpPut("{announcementId:guid}/schedule")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Schedule course announcement")]
        [EndpointDescription("Sets a scheduled publish time for a draft announcement. Instructor only.")]
        public async Task<IActionResult> ScheduleCourseAnnouncement(Guid courseId, Guid announcementId, ScheduleCourseAnnouncementRequest request)
        {
            var command = new ScheduleCourseAnnouncementCommand(courseId, announcementId, GetUserId(), request.ScheduledAt);
            var result = await _sender.Send(command);
            return await ToNoContentResultAsync(result);
        }

        [HttpGet("{announcementId:guid}")]
        public IActionResult GetById(Guid courseId, Guid announcementId)
        {
            return Ok();
        }
    }

}
