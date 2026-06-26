using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Payments;
using MediatR;

namespace MasarHub.Application.Features.Payments.Commands.InitiatePayment
{
    public sealed record InitiatePaymentCommand
    (
        Guid UserId,
        Guid OrderId,
        PaymentProvider Provider
    ) : IRequest<Result<InitiatePaymentResponse>>;
}
