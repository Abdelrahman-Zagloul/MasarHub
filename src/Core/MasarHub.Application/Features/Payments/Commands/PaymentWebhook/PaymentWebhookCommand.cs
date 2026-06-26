using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Payments;
using MediatR;

namespace MasarHub.Application.Features.Payments.Commands.PaymentWebhook
{
    public sealed record PaymentWebhookCommand
    (
        PaymentProvider Provider,
        string RawBody,
        Dictionary<string, string> Headers
    ) : IRequest<Result<PaymentWebhookResponse>>;
}
