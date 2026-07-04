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
    [AllowAnonymous]
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

        [HttpPost("stripe/payments")]
        [EndpointSummary("Stripe Payment webhook")]
        [EndpointDescription("Receives payment result from the payment provider(Stripe) and updates the order.")]
        public async Task<IActionResult> StripePaymentWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();
            var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            var result = await _sender.Send(new PaymentWebhookCommand(PaymentProvider.Stripe, rawBody, headers));
            return result.IsFailure
                ? await HandleError(result)
                : Ok(result.Value);
        }


        [HttpPost("paymob/payments")]
        [EndpointSummary("Paymob Payment webhook")]
        [EndpointDescription("Receives payment result from the payment provider(Paymob) and updates the order.")]
        public async Task<IActionResult> PaymobPaymentWebhook(string hmac)
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();
            var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            if (!headers.ContainsKey("hmac") && !string.IsNullOrEmpty(hmac))
                headers.Add("hmac", hmac);

            var result = await _sender.Send(new PaymentWebhookCommand(PaymentProvider.Paymob, rawBody, headers));
            return result.IsFailure
                ? await HandleError(result)
                : Ok(result.Value);
        }
    }
}
