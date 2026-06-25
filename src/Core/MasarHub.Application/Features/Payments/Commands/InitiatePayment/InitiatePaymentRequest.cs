using MasarHub.Domain.Modules.Payments;

namespace MasarHub.Application.Features.Payments.Commands.InitiatePayment
{
    public sealed record InitiatePaymentRequest(PaymentProvider Provider);
}
