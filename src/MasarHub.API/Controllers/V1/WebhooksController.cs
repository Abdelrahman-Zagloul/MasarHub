using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Features.Payments.Commands.PaymentWebhook;
using MasarHub.Domain.Modules.Payments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Webhooks")]
    [Route("api/v{version:apiVersion}/webhooks")]
    public sealed class WebhooksController : ApiControllerBase
    {
        private readonly ISender _sender;

        public WebhooksController(ILocalizationService localizationService, ISender sender) : base(localizationService)
        {
            _sender = sender;
        }

        [AllowAnonymous]
        [HttpPost("{provider}/payments")]
        [EndpointSummary("Payment webhook")]
        [EndpointDescription("Receives payment result from the payment provider and updates the order.")]
        public async Task<IActionResult> PaymentWebhook([FromRoute] PaymentProvider provider)
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();
            var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var result = await _sender.Send(new PaymentWebhookCommand(provider, rawBody, headers));
            return result.IsFailure
                ? await HandleError(result)
                : Ok(result.Value);
        }
    }
}
