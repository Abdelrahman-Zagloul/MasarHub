using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Settings;
using MasarHub.Domain.Modules.Orders;
using MasarHub.Domain.Modules.Payments;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MasarHub.Infrastructure.Services
{
    public class PaymobPaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobSettings _settings;
        private readonly ILogger<PaymobPaymentService> _logger;

        public PaymentProvider Provider => PaymentProvider.Paymob;

        public PaymobPaymentService(HttpClient httpClient, IOptions<PaymobSettings> settings, ILogger<PaymobPaymentService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;

            _httpClient.BaseAddress = new Uri("https://accept.paymob.com/");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _settings.SecretKey);
        }

        public async Task<Result<PaymentCreationResult>> CreateSessionAsync(Order order, IReadOnlyCollection<OrderItem> items, CancellationToken ct = default)
        {
            try
            {
                var requestBody = new
                {
                    amount = (int)(order.FinalAmount * 100),
                    currency = "EGP",
                    payment_methods = new[] { _settings.IntegrationId },
                    items = items.Select(i => new
                    {
                        name = i.CourseTitle,
                        amount = (int)(i.FinalPrice * 100),
                        quantity = 1,
                        description = i.CourseTitle,
                    }).ToList(),
                    billing_data = new
                    {
                        first_name = "NA",
                        last_name = "NA",
                        email = "fix@example.com",
                        phone_number = "NA",
                        apartment = "NA",
                        floor = "NA",
                        street = "NA",
                        building = "NA",
                        shipping_method = "NA",
                        postal_code = "NA",
                        city = "NA",
                        country = "EG",
                        state = "NA"
                    },
                    special_reference = order.Id.ToString(),
                    expiration = 3600
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("v1/intention/", jsonContent, ct);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(ct);
                    _logger.LogError("Paymob intention creation failed: {StatusCode} {Error}", response.StatusCode, errorContent);
                    return new Error("paymob.intention_creation_failed", ErrorType.Failure, new() { ["OrderId"] = order.Id.ToString() });
                }

                var responseString = await response.Content.ReadAsStringAsync(ct);
                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                var intentionId = root.GetProperty("id").GetString()!;
                var clientSecret = root.GetProperty("client_secret").GetString()!;
                var checkoutUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={_settings.PublicKey}&clientSecret={clientSecret}";

                _logger.LogInformation("Paymob intention created for order {OrderId}: intention={IntentionId}", order.Id, intentionId);
                return new PaymentCreationResult(intentionId, checkoutUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Paymob session creation failed for order {OrderId}", order.Id);
                return new Error("paymob.session_creation_failed", ErrorType.Failure, new() { ["OrderId"] = order.Id.ToString() });
            }
        }

        public async Task<Result<PaymentWebhookValidationResult>> ValidateWebhookAsync(string rawBody, IDictionary<string, string> headers, CancellationToken ct = default)
        {
            if (!headers.TryGetValue("hmac", out var hmacHeader) && !headers.TryGetValue("Hmac", out hmacHeader))
                return Error.Failure("paymob.webhook_missing_hmac");

            try
            {
                using var doc = JsonDocument.Parse(rawBody);
                var root = doc.RootElement;

                if (!root.TryGetProperty("obj", out var obj))
                    return Error.Failure("paymob.webhook_invalid_payload");

                if (!ValidateHmac(obj, hmacHeader))
                {
                    _logger.LogWarning("Paymob webhook HMAC validation failed");
                    return Error.Failure("paymob.webhook_invalid_hmac");
                }

                var transactionId = obj.GetProperty("id").GetInt64();
                _logger.LogInformation("Paymob webhook received: transaction={TransactionId}", transactionId);

                var success = obj.GetProperty("success").GetBoolean();
                var pending = obj.GetProperty("pending").GetBoolean();
                var isVoided = obj.GetProperty("is_voided").GetBoolean();
                var isRefunded = obj.GetProperty("is_refunded").GetBoolean();

                var status = (success, pending, isVoided, isRefunded) switch
                {
                    (true, _, false, false) => PaymentStatus.Succeeded,
                    (false, true, false, false) => PaymentStatus.Pending,
                    (_, _, true, _) => PaymentStatus.Cancelled,
                    (_, _, _, true) => PaymentStatus.Cancelled,
                    _ => PaymentStatus.Failed
                };

                return new PaymentWebhookValidationResult(transactionId.ToString(), status);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Paymob webhook invalid JSON");
                return Error.Failure("paymob.webhook_invalid_json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Paymob webhook validation failed");
                return Error.Failure("paymob.webhook_validation_failed");
            }
        }

        private bool ValidateHmac(JsonElement obj, string receivedHmac)
        {
            try
            {
                var fields = new[]
                {
                    "amount_cents", "created_at", "currency", "error_occured",
                    "has_parent_transaction", "id", "integration_id", "is_3d_secure",
                    "is_auth", "is_capture", "is_refunded", "is_standalone_payment",
                    "is_voided", "order.id", "owner", "pending",
                    "source_data.pan", "source_data.sub_type", "source_data.type", "success"
                };

                var values = fields.Select(f => GetNestedValue(obj, f));
                var concatenated = string.Join("", values);

                var keyBytes = Encoding.UTF8.GetBytes(_settings.HMAC);
                var hash = HMACSHA512.HashData(keyBytes, Encoding.UTF8.GetBytes(concatenated));
                var calculated = Convert.ToHexString(hash).ToLowerInvariant();

                return string.Equals(calculated, receivedHmac, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static string GetNestedValue(JsonElement element, string path)
        {
            var parts = path.Split('.');
            var current = element;

            foreach (var part in parts)
            {
                if (!current.TryGetProperty(part, out var next))
                    return string.Empty;

                current = next;
            }

            return current.ValueKind switch
            {
                JsonValueKind.String => current.GetString() ?? string.Empty,
                JsonValueKind.Number => current.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => string.Empty
            };
        }
    }
}
