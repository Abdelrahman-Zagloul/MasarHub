using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Orders;
using MasarHub.Domain.Modules.Payments;
using MediatR;

namespace MasarHub.Application.Features.Payments.Commands.PaymentWebhook
{
    public sealed class PaymentWebhookCommandHandler : IRequestHandler<PaymentWebhookCommand, Result<PaymentWebhookResponse>>
    {
        private readonly IEnumerable<IPaymentService> _paymentStrategies;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<CourseEnrollment> _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentWebhookCommandHandler(IEnumerable<IPaymentService> paymentStrategies, IRepository<Payment> paymentRepository, IRepository<Order> orderRepository, IRepository<CourseEnrollment> enrollmentRepository, IUnitOfWork unitOfWork)
        {
            _paymentStrategies = paymentStrategies;
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PaymentWebhookResponse>> Handle(PaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            var gateway = _paymentStrategies.FirstOrDefault(s => s.Provider == request.Provider);
            if (gateway == null)
                return Error.NotFound("payment.provider_not_supported");

            var webhookResult = await gateway.ValidateWebhookAsync(request.RawBody, request.Headers, cancellationToken);
            if (webhookResult.IsFailure)
                return webhookResult.Errors[0];

            var payment = await _paymentRepository.GetAsync(p => p.ProviderReference == webhookResult.Value.ProviderReference && p.Provider == request.Provider, cancellationToken);
            if (payment == null)
                return Error.NotFound("payment.not_found");

            if (webhookResult.Value.Status == PaymentStatus.Succeeded)
            {
                var markResult = payment.MarkSucceeded();
                if (markResult.IsFailure)
                {
                    // handle Idempotency
                    if (markResult.Error == PaymentErrors.CannotChangeFinalPaymentStatus && payment.Status == PaymentStatus.Succeeded)
                        return new PaymentWebhookResponse(payment.Id, payment.OrderId, payment.Status);

                    return markResult.Error;
                }

                var order = await _orderRepository.GetAsync(x => x.Id == payment.OrderId, cancellationToken, x => x.Items);
                if (order == null)
                    return Error.NotFound(OrderErrors.NotFound);

                var paidResult = order.MarkPaid();
                if (paidResult.IsFailure)
                    return paidResult.Error;

                foreach (var item in order.Items)
                {
                    var enrollmentResult = CourseEnrollment.Create(order.UserId, item.CourseId, order.Id, item.FinalPrice);
                    if (enrollmentResult.IsFailure)
                        return enrollmentResult.Error;

                    await _enrollmentRepository.AddAsync(enrollmentResult.Value, cancellationToken);
                }
            }
            else if (webhookResult.Value.Status == PaymentStatus.Expired)
            {
                var markResult = payment.MarkExpired();
                if (markResult.IsFailure)
                    return markResult.Error;
            }
            else
            {
                var markResult = payment.MarkFailed();
                if (markResult.IsFailure)
                    return markResult.Error;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new PaymentWebhookResponse(payment.Id, payment.OrderId, webhookResult.Value.Status);
        }
    }
}
