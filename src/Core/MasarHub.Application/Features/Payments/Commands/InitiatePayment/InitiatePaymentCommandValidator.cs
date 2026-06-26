using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Payments.Commands.InitiatePayment
{
    public sealed class InitiatePaymentCommandValidator : AbstractValidator<InitiatePaymentCommand>
    {
        public InitiatePaymentCommandValidator()
        {
            RuleFor(x => x.UserId)
                .ValidGuid("UserId");

            RuleFor(x => x.OrderId)
                .ValidGuid("OrderId");

            RuleFor(x => x.Provider)
                .ValidEnum("Provider");
        }
    }
}
