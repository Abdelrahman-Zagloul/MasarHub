using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Coupons.Commands.CreateCoupon;
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

        [HttpPost]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Create a coupon")]
        [EndpointDescription("Creates a new coupon for a specific course. Instructor only.")]
        public async Task<IActionResult> CreateCoupon(Guid courseId, CreateCouponRequest request)
        {
            var command = new CreateCouponCommand(courseId, GetUserId(), request.Code, request.Value, request.Type, request.ExpirationDate, request.UsageLimit);
            var result = await _sender.Send(command);

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetCouponById), new { courseId, result.Value.Id }, result.Value);

        }

        public IActionResult GetCouponById(Guid courseId, Guid couponId)
        {
            return Ok();
        }
    }
}
