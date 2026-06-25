namespace MasarHub.Application.Features.Payments.Commands.InitiatePayment
{
    public sealed record InitiatePaymentResponse(Guid PaymentId, string PaymentUrl);
}
