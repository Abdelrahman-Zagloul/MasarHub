namespace MasarHub.Application.Features.Payments.Commands.PaymentWebhook
{
    public sealed record PaymentWebhookRequest(Guid PaymentId, string Status);
}
