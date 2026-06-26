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

            var paymentStatus = webhookResult.Value.Status;
            var payment = await _paymentRepository.GetAsync(p => p.ProviderReference == webhookResult.Value.ProviderReference && p.Provider == request.Provider, cancellationToken);
            if (payment == null)
                return Error.NotFound("payment.not_found");

            if (paymentStatus == PaymentStatus.Pending)
                return new PaymentWebhookResponse(payment.Id, payment.OrderId, paymentStatus);

            var processResult = paymentStatus switch
            {
                PaymentStatus.Succeeded => await HandleSucceededPaymentAsync(payment, cancellationToken),
                PaymentStatus.Failed or PaymentStatus.Cancelled => await HandleFailedPaymentAsync(payment, cancellationToken),
                PaymentStatus.Expired => HandleExpiredPayment(payment),
                _ => Result.Success()
            };

            if (processResult.IsFailure)
                return processResult.Errors[0];

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new PaymentWebhookResponse(payment.Id, payment.OrderId, paymentStatus);
        }

        private async Task<Result> HandleSucceededPaymentAsync(Payment payment, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetAsync(x => x.Id == payment.OrderId, cancellationToken, x => x.Items);
            if (order == null)
                return Error.NotFound(OrderErrors.NotFound);

            var markSucceededResult = payment.MarkSucceeded(order.UserId, order.OrderNumber);
            if (markSucceededResult.IsFailure)
            {
                // Handle Idempotency
                if (markSucceededResult.Error == PaymentErrors.CannotChangeFinalPaymentStatus && payment.Status == PaymentStatus.Succeeded)
                    return Result.Success();

                return markSucceededResult.Error;
            }

            var paidResult = order.MarkPaid();
            if (paidResult.IsFailure)
                return paidResult.Error;

            foreach (var item in order.Items)
            {
                var enrollmentResult = CourseEnrollment.Create(order.UserId, item.CourseId, item.CourseTitle, order.Id, item.FinalPrice);
                if (enrollmentResult.IsFailure)
                    return enrollmentResult.Error;

                await _enrollmentRepository.AddAsync(enrollmentResult.Value, cancellationToken);
            }
            return Result.Success();
        }
        private async Task<Result> HandleFailedPaymentAsync(Payment payment, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(payment.OrderId, cancellationToken);
            if (order == null)
                return Error.NotFound(OrderErrors.NotFound);

            var markFailedResult = payment.MarkFailed(order.UserId, order.OrderNumber);
            if (markFailedResult.IsFailure)
                return markFailedResult.Error;

            return Result.Success();
        }
        private Result HandleExpiredPayment(Payment payment)
        {
            var markExpiredResult = payment.MarkExpired();
            if (markExpiredResult.IsFailure)
                return markExpiredResult.Error;

            return Result.Success();
        }
    }
}
