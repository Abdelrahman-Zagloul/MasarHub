using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Orders.Commands.CancelOrder;
using MasarHub.Application.Features.Orders.Commands.CreateOrder;
using MasarHub.Application.Features.Orders.Commands.DeleteOrder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Orders")]
    [Route("api/v{version:apiVersion}/orders")]
    public sealed class OrdersController : ApiControllerBase
    {
        private readonly ISender _sender;

        public OrdersController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Student)]
        [EndpointSummary("Create an order")]
        [EndpointDescription("Creates an order from the current user's cart. Optionally applies a coupon.")]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            var command = new CreateOrderCommand(GetUserId(), request.CourseCoupons);
            var result = await _sender.Send(command);

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetOrderById), new { orderId = result.Value.Id }, result.Value);
        }

        [HttpPut("{orderId:guid}/cancel")]
        [Authorize(Roles = Roles.Student)]
        [EndpointSummary("Cancel an order")]
        [EndpointDescription("Cancels a pending order.")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            var result = await _sender.Send(new CancelOrderCommand(GetUserId(), orderId));
            return await ToNoContentResultAsync(result);
        }

        [HttpDelete("{orderId:guid}")]
        [Authorize(Roles = Roles.Student)]
        [EndpointSummary("Delete an order")]
        [EndpointDescription("Delete a pending order.")]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            var result = await _sender.Send(new DeleteOrderCommand(GetUserId(), orderId));
            return await ToNoContentResultAsync(result);
        }

        [HttpGet("{orderId:guid}")]
        public IActionResult GetOrderById(Guid orderId)
        {
            return Ok();
        }
    }
}
