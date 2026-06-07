using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Courses.Commands.CreateCourse;
using MasarHub.Application.Features.Courses.Commands.UpdateCourseLearningObjective;
using MasarHub.Application.Features.Courses.Commands.UpdateCoursePrerequisites;
using MasarHub.Application.Features.Courses.Commands.UpdateCourseRequirements;
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
            return Ok();
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
    }
}
