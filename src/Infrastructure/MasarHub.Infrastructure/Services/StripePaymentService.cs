using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Settings;
using MasarHub.Domain.Modules.Orders;
using MasarHub.Domain.Modules.Payments;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace MasarHub.Infrastructure.Services
{
    public sealed class StripePaymentService : IPaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly ILogger<StripePaymentService> _logger;

        public StripePaymentService(IOptions<StripeSettings> options, ILogger<StripePaymentService> logger)
        {
            _stripeSettings = options.Value;
            _logger = logger;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public PaymentProvider Provider => PaymentProvider.Stripe;

        public async Task<Result<PaymentCreationResult>> CreateSessionAsync(Order order, IReadOnlyCollection<OrderItem> items, CancellationToken ct = default)
        {
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = _stripeSettings.SuccessUrl,
                CancelUrl = _stripeSettings.CancelUrl,
                ClientReferenceId = order.Id.ToString(),
                LineItems = items.Select(item => new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmountDecimal = item.FinalPrice * 100m,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.CourseTitle,
                        }
                    },
                }).ToList(),
            };

            try
            {
                var service = new SessionService();
                var session = await service.CreateAsync(options, cancellationToken: ct);

                _logger.LogInformation("Stripe session created for order {OrderId}: session {SessionId}", order.Id, session.Id);
                return new PaymentCreationResult(session.Id, session.Url);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe session creation failed for order {OrderId}", order.Id);
                return Error.Failure("stripe.session_creation_failed");
            }
        }

        public async Task<Result<PaymentWebhookValidationResult>> ValidateWebhookAsync(string rawBody, IDictionary<string, string> headers, CancellationToken ct = default)
        {
            if (!headers.TryGetValue("Stripe-Signature", out var signature))
                return Error.Failure("stripe.webhook_missing_signature");

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(rawBody, signature, _stripeSettings.WebhookSecret, throwOnApiVersionMismatch: false);
                _logger.LogInformation("Stripe webhook received: type={EventType}, id={EventId}", stripeEvent.Type, stripeEvent.Id);

                var allowedEvents = new[]
                {
                    EventTypes.CheckoutSessionCompleted,
                    EventTypes.CheckoutSessionAsyncPaymentSucceeded,
                    EventTypes.CheckoutSessionExpired,
                    EventTypes.CheckoutSessionAsyncPaymentFailed
                };

                if (!allowedEvents.Contains(stripeEvent.Type))
                {
                    _logger.LogInformation("Stripe event {EventType} safely ignored.", stripeEvent.Type);
                    return new PaymentWebhookValidationResult(string.Empty, PaymentStatus.Pending);
                }

                if (stripeEvent.Data.Object is not Session session)
                {
                    _logger.LogWarning("Stripe webhook event {EventType} did not contain a Session object", stripeEvent.Type);
                    return Error.Failure("stripe.webhook_invalid_session");
                }

                return stripeEvent.Type switch
                {
                    EventTypes.CheckoutSessionCompleted => session.PaymentStatus switch
                    {
                        "paid" => new PaymentWebhookValidationResult(session.Id, PaymentStatus.Succeeded),
                        "no_payment_required" => new PaymentWebhookValidationResult(session.Id, PaymentStatus.Succeeded),
                        "unpaid" => new PaymentWebhookValidationResult(session.Id, PaymentStatus.Pending),
                        _ => Error.Failure("stripe.webhook_unknown_payment_status")
                    },

                    EventTypes.CheckoutSessionAsyncPaymentSucceeded => new PaymentWebhookValidationResult(session.Id, PaymentStatus.Succeeded),
                    EventTypes.CheckoutSessionExpired => new PaymentWebhookValidationResult(session.Id, PaymentStatus.Expired),
                    EventTypes.CheckoutSessionAsyncPaymentFailed => new PaymentWebhookValidationResult(session.Id, PaymentStatus.Failed),
                    _ => new PaymentWebhookValidationResult(string.Empty, PaymentStatus.Pending)
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook validation failed");
                return Error.Failure("stripe.webhook_validation_failed");
            }
        }
    }
}
