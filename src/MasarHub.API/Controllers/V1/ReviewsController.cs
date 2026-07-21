using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Reviews.Commands.CreateCourseReview;
using MasarHub.Application.Features.Reviews.Commands.DeleteCourseReview;
using MasarHub.Application.Features.Reviews.Commands.UpdateCourseReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Reviews")]
    [Route("api/v{version:apiVersion}/courses/{courseId:guid}/reviews")]
    public sealed class ReviewsController : ApiControllerBase
    {
        private readonly ISender _sender;

        public ReviewsController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Student)]
        [EndpointSummary("Create course review")]
        [EndpointDescription("Creates a rating and review for a course. Student must be enrolled.")]
        public async Task<IActionResult> CreateCourseReview(Guid courseId, CreateCourseReviewRequest request)
        {
            var command = new CreateCourseReviewCommand(courseId, GetUserId(), request.Rating, request.ReviewContent);
            var result = await _sender.Send(command);
            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(null, new { courseId }, result.Value);
        }

        [HttpPut("{reviewId:guid}")]
        [Authorize(Roles = Roles.Student)]
        [EndpointSummary("Update course review")]
        [EndpointDescription("Updates a rating and/or review content. Student must own the review.")]
        public async Task<IActionResult> UpdateCourseReview(Guid courseId, Guid reviewId, UpdateCourseReviewRequest request)
        {
            var command = new UpdateCourseReviewCommand(courseId, reviewId, GetUserId(), request.Rating, request.ReviewContent);
            var result = await _sender.Send(command);
            return await ToNoContentResultAsync(result);
        }

        [HttpDelete("{reviewId:guid}")]
        [Authorize(Roles = Roles.Student)]
        [EndpointSummary("Delete course review")]
        [EndpointDescription("Soft deletes a course review. Student must own the review.")]
        public async Task<IActionResult> DeleteCourseReview(Guid courseId, Guid reviewId)
        {
            var command = new DeleteCourseReviewCommand(courseId, reviewId, GetUserId());
            var result = await _sender.Send(command);
            return await ToNoContentResultAsync(result);
        }
    }
}
