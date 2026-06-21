using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Carts.Commands.AddToCart;
using MasarHub.Application.Features.Carts.Commands.ClearCart;
using MasarHub.Application.Features.Carts.Commands.RemoveFromCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Carts")]
    [Route("api/v{version:apiVersion}/cart")]
    public sealed class CartsController : ApiControllerBase
    {
        private readonly ISender _sender;

        public CartsController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }

        [HttpPut("items/{courseId:guid}")]
        [Authorize(Roles = Roles.Student)]
        [EndpointSummary("Add course to cart")]
        [EndpointDescription("Adds a published course to the current user's cart.")]
        public async Task<IActionResult> AddToCart(Guid courseId)
        {
            var result = await _sender.Send(new AddToCartCommand(courseId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }

        [HttpDelete("items/{courseId:guid}")]
        [Authorize(Roles = Roles.Student)]
        [EndpointSummary("Remove course from cart")]
        [EndpointDescription("Removes a course from the current user's cart.")]
        public async Task<IActionResult> RemoveFromCart(Guid courseId)
        {
            var result = await _sender.Send(new RemoveFromCartCommand(courseId, GetUserId()));
            return await ToNoContentResultAsync(result);
        }

        [HttpDelete]
        [Authorize(Roles = Roles.Student)]
        [EndpointSummary("Clear cart")]
        [EndpointDescription("Removes all courses from the current user's cart.")]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _sender.Send(new ClearCartCommand(GetUserId()));
            return await ToNoContentResultAsync(result);
        }
    }
}