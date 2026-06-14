using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.API.Extensions.Mappers;
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
using MasarHub.Application.Features.Courses.Queries.GetCourses;
using MasarHub.Application.Features.Courses.Queries.GetCourseThumbnail;
using MasarHub.Application.Features.Courses.Queries.GetInstructorCourses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Courses")]
    [Route("api/v{version:apiVersion}/courses")]
    public sealed class CoursesController : ApiControllerBase
    {
        private readonly ISender _sender;
        public CoursesController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }


        [HttpPost]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Create a new course")]
        [EndpointDescription("Creates a new course as a draft with the specified title, description, price, language, level, and category. Instructor only.")]
        public async Task<IActionResult> CreateCourse(CreateCourseCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetCourseById), new { id = result.Value.Id }, result.Value);
        }


        [HttpGet("{id:guid}")]
        [EndpointSummary("Get course by ID")]
        [EndpointDescription("Retrieves detailed information about a specific course including its modules, lessons, and metadata.")]
        public async Task<IActionResult> GetCourseById(Guid id)
        {
            var result = await _sender.Send(new GetCourseByIdQuery(id));
            return await ToOkResultAsync(result);
        }


        [HttpGet]
        [EndpointSummary("List all courses")]
        [EndpointDescription("Retrieves a paginated list of all courses with optional filtering, sorting, and search capabilities.")]
        public async Task<IActionResult> GetCourses([FromQuery] GetCoursesQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            return await ToOkResultAsync(result);
        }


        [HttpGet("instructor/me")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Get my instructor courses")]
        [EndpointDescription("Retrieves all courses owned by the authenticated instructor with their current status and statistics.")]
        public async Task<IActionResult> GetInstructorCourses([FromQuery] GetInstructorCoursesQuery query)
        {
            var result = await _sender.Send(query);
            return await ToOkResultAsync(result);
        }


        [HttpGet("{id:guid}/thumbnail")]
        [EndpointSummary("Get course thumbnail URL")]
        [EndpointDescription("Retrieves the URL of the course thumbnail image.")]
        public async Task<IActionResult> GetThumbnail(Guid id)
        {
            var result = await _sender.Send(new GetCourseThumbnailQuery(id));
            return await ToOkResultAsync(result);
        }


        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Update course details")]
        [EndpointDescription("Updates the title, description, price, language, level, and category of an existing course. Instructor only.")]
        public async Task<IActionResult> UpdateCourse(Guid id, UpdateCourseRequest request)
        {
            var result = await _sender.Send(
                new UpdateCourseCommand(id, request.Title, request.Description, request.Price, request.Language, request.Level, request.CategoryId));

            return await ToNoContentResultAsync(result);
        }


        [HttpPut("{id:guid}/prerequisites")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Update course prerequisites")]
        [EndpointDescription("Updates the list of prerequisites required for the course. Instructor only.")]
        public async Task<IActionResult> UpdateCoursePrerequisites(Guid id, UpdateCoursePrerequisitesRequest request)
        {
            var result = await _sender.Send(new UpdateCoursePrerequisitesCommand(id, request.Prerequisites));

            return await ToNoContentResultAsync(result);
        }


        [HttpPut("{id:guid}/requirements")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Update course requirements")]
        [EndpointDescription("Updates the list of requirements or materials needed for the course. Instructor only.")]
        public async Task<IActionResult> UpdateCourseRequirements(Guid id, UpdateCourseRequirementsRequest request)
        {
            var result = await _sender.Send(new UpdateCourseRequirementsCommand(id, request.Requirements));
            return await ToNoContentResultAsync(result);
        }


        [HttpPut("{id:guid}/learning-objectives")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Update course learning objectives")]
        [EndpointDescription("Updates the learning objectives for the course. Instructor only.")]
        public async Task<IActionResult> UpdateCourseLearningObjectives(Guid id, UpdateCourseLearningObjectiveRequest request)
        {
            var result = await _sender.Send(new UpdateCourseLearningObjectiveCommand(id, request.LearningObjectives));
            return await ToNoContentResultAsync(result);
        }


        [HttpPut("{id:guid}/thumbnail")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Update course thumbnail")]
        [EndpointDescription("Uploads a new thumbnail image for the course. The old thumbnail is automatically cleaned up. Instructor only.")]
        public async Task<IActionResult> UpdateThumbnail(Guid id, IFormFile file)
        {
            var result = await _sender.Send(new UpdateCourseThumbnailCommand(id, file.ToResource()));
            return await ToOkResultAsync(result, new { ThumbnailUrl = result.Value });
        }


        [HttpPut("{id:guid}/submit")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Submit course for approval")]
        [EndpointDescription("Submits the course for admin review and approval. The course must be complete before submission. Instructor only.")]
        public async Task<IActionResult> SubmitCourseForApproval(Guid id)
        {
            var result = await _sender.Send(new SubmitCourseForApprovalCommand(id, GetUserId()));
            return await ToNoContentResultAsync(result);
        }


        [HttpPut("{id:guid}/approve")]
        [Authorize(Roles = Roles.Admin)]
        [EndpointSummary("Approve a submitted course")]
        [EndpointDescription("Approves a submitted course, making it published and visible to students. Admin only.")]
        public async Task<IActionResult> ApproveCourse(Guid id)
        {
            var result = await _sender.Send(new ApproveCourseCommand(id, GetUserId()));
            return await ToNoContentResultAsync(result);
        }


        [HttpPut("{id:guid}/reject")]
        [Authorize(Roles = Roles.Admin)]
        [EndpointSummary("Reject a submitted course")]
        [EndpointDescription("Rejects a submitted course with a reason for rejection. The course will be returned to draft status. Admin only.")]
        public async Task<IActionResult> RejectCourse(Guid id, RejectCourseRequest request, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new RejectCourseCommand(id, GetUserId(), request.Reason), cancellationToken);
            return await ToNoContentResultAsync(result);
        }
    }
}