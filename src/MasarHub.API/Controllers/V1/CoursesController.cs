using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Courses.Commands.ApproveCourse;
using MasarHub.Application.Features.Courses.Commands.CreateCourse;
using MasarHub.Application.Features.Courses.Commands.RejectCourse;
using MasarHub.Application.Features.Courses.Commands.SubmitCourseForApproval;
using MasarHub.Application.Features.Courses.Commands.UpdateCourse;
using MasarHub.Application.Features.Courses.Commands.UpdateCourseLearningObjective;
using MasarHub.Application.Features.Courses.Commands.UpdateCoursePrerequisites;
using MasarHub.Application.Features.Courses.Commands.UpdateCourseRequirements;
using MasarHub.Application.Features.Courses.Commands.UpdateCourseThumbnail;
using MasarHub.Application.Features.Courses.Queries.GetCourseById;
using MasarHub.Application.Features.Courses.Queries.GetCourseThumbnail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Courses")]
    [Route("api/courses")]
    public sealed class CoursesController : ApiBaseController
    {
        private readonly IMediator _mediator;
        public CoursesController(ILocalizationService localizationService, IMediator mediator) : base(localizationService)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> CreateCourse(CreateCourseCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return await HandleError(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, new
            {
                Message = await _localizationService.GetAsync("course.created"),
                Course = result.Value
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCourseByIdQuery(id));
            if (result.IsFailure)
                return await HandleError(result);

            return Ok(result.Value);
        }

        [HttpGet("{id:guid}/thumbnail")]
        public async Task<IActionResult> GetThumbnail(Guid id)
        {
            var result = await _mediator.Send(new GetCourseThumbnailQuery(id));
            if (result.IsFailure)
                return await HandleError(result);

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> UpdateCourse(Guid id, UpdateCourseRequest request)
        {
            var result = await _mediator.Send(
                new UpdateCourseCommand(id, request.Title, request.Description, request.Price, request.Language, request.Level, request.CategoryId));
            if (result.IsFailure)
                return await HandleError(result);

            return NoContent();
        }

        [HttpPut("{id:guid}/prerequisites")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> UpdateCoursePrerequisites(Guid id, UpdateCoursePrerequisitesRequest request)
        {
            var result = await _mediator.Send(new UpdateCoursePrerequisitesCommand(id, request.Prerequisites));
            if (result.IsFailure)
                return await HandleError(result);

            return NoContent();
        }

        [HttpPut("{id:guid}/requirements")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> UpdateCourseRequirements(Guid id, UpdateCourseRequirementsRequest request)
        {
            var result = await _mediator.Send(new UpdateCourseRequirementsCommand(id, request.Requirements));
            if (result.IsFailure)
                return await HandleError(result);

            return NoContent();
        }

        [HttpPut("{id:guid}/learning-objectives")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> UpdateCourseLearningObjectives(Guid id, UpdateCourseLearningObjectiveRequest request)
        {
            var result = await _mediator.Send(new UpdateCourseLearningObjectiveCommand(id, request.LearningObjectives));
            if (result.IsFailure)
                return await HandleError(result);

            return NoContent();
        }

        [HttpPut("{id:guid}/thumbnail")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> UpdateThumbnail(Guid id, IFormFile file)
        {
            var fileResource = new FileResource(file.FileName, file.ContentType, file.OpenReadStream(), file.Length);

            var result = await _mediator.Send(new UpdateCourseThumbnailCommand(id, fileResource));
            if (result.IsFailure)
                return await HandleError(result);

            return Ok(new { ThumbnailUrl = result.Value });
        }

        [HttpPut("{id:guid}/submit")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> SubmitCourseForApproval(Guid id)
        {
            var result = await _mediator.Send(new SubmitCourseForApprovalCommand(id, GetUserId()));
            if (result.IsFailure)
                return await HandleError(result);

            return NoContent();
        }

        [HttpPut("{id:guid}/approve")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ApproveCourse(Guid id)
        {
            var result = await _mediator.Send(new ApproveCourseCommand(id, GetUserId()));
            if (result.IsFailure)
                return await HandleError(result);

            return NoContent();
        }

        [HttpPut("{id:guid}/reject")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RejectCourse(Guid id, RejectCourseRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RejectCourseCommand(id, GetUserId(), request.Reason), cancellationToken);
            if (result.IsFailure)
                return await HandleError(result);

            return NoContent();
        }
    }
}