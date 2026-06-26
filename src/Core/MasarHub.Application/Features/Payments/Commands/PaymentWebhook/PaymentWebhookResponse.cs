using MasarHub.Domain.Modules.Payments;

namespace MasarHub.Application.Features.Payments.Commands.PaymentWebhook
{
    public sealed record PaymentWebhookResponse(Guid PaymentId, Guid OrderId, PaymentStatus Status);
}
