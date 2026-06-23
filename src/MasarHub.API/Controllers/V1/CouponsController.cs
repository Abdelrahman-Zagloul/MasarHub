using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Coupons.Commands.CreateCoupon;
using MasarHub.Application.Features.Coupons.Commands.DeleteCoupon;
using MasarHub.Application.Features.Coupons.Commands.UpdateCoupon;
using MasarHub.Application.Features.Coupons.Queries.GetCouponById;
using MasarHub.Application.Features.Coupons.Queries.GetCoupons;
using MasarHub.Domain.Modules.Payments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Coupons")]
    [Route("api/v{version:apiVersion}/courses/{courseId}/coupons")]
    public sealed class CouponsController : ApiControllerBase
    {
        private readonly ISender _sender;

        public CouponsController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }

        [HttpPost()]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Create a coupon")]
        [EndpointDescription("Creates a new coupon for a specific course. Instructor only.")]
        public async Task<IActionResult> CreateCoupon(Guid courseId, CreateCouponRequest request)
        {
            var command = new CreateCouponCommand(courseId, GetUserId(), request.Code, request.Value, request.Type, request.ExpirationDate, request.UsageLimit);
            var result = await _sender.Send(command);

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetCouponById), new { courseId, couponId = result.Value.Id }, result.Value);

        }


        [HttpPut("{couponId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Update a coupon")]
        [EndpointDescription("updates a coupon's value, expiration date, or usage limit. Instructor must own the associated course.")]
        public async Task<IActionResult> UpdateCoupon(Guid courseId, Guid couponId, UpdateCouponRequest request)
        {
            var command = new UpdateCouponCommand(courseId, couponId, GetUserId(), request.Value, request.ExpirationDate, request.UsageLimit);
            var result = await _sender.Send(command);

            return await ToNoContentResultAsync(result);
        }

        [HttpDelete("{couponId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Delete a coupon")]
        [EndpointDescription("Deletes a coupon by ID. Instructor must own the associated course.")]
        public async Task<IActionResult> DeleteCoupon(Guid courseId, Guid couponId)
        {
            var result = await _sender.Send(new DeleteCouponCommand(courseId, couponId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }


        [HttpGet]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("List coupons")]
        [EndpointDescription("Returns a paginated list of coupons for the specified course. Instructor only.")]
        public async Task<IActionResult> GetCoupons(Guid courseId, CouponStatus? status = null)
        {
            var result = await _sender.Send(new GetCouponsQuery(courseId, GetUserId(), status));
            return await ToOkResultAsync(result);
        }

        [HttpGet("{couponId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Get coupon by ID")]
        [EndpointDescription("Returns coupon details. Instructor must own the associated course.")]
        public async Task<IActionResult> GetCouponById(Guid courseId, Guid couponId)
        {
            var result = await _sender.Send(new GetCouponByIdQuery(courseId, couponId, GetUserId()));
            return await ToOkResultAsync(result);
        }
    }
}
