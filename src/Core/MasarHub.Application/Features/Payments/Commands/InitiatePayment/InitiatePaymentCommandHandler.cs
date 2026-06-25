using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Orders;
using MasarHub.Domain.Modules.Payments;
using MediatR;

namespace MasarHub.Application.Features.Payments.Commands.InitiatePayment
{
    public sealed class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, Result<InitiatePaymentResponse>>
    {
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IOrderQuery _orderQuery;
        private readonly IEnumerable<IPaymentService> _paymentStrategies;
        private readonly IUnitOfWork _unitOfWork;

        public InitiatePaymentCommandHandler(IRepository<Payment> paymentRepository, IOrderQuery orderQuery, IEnumerable<IPaymentService> paymentStrategies, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _orderQuery = orderQuery;
            _paymentStrategies = paymentStrategies;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<InitiatePaymentResponse>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderQuery.GetOrderAsync(request.OrderId, request.UserId, cancellationToken);
            if (order == null)
                return Error.NotFound("order.not_found");

            if (order.Status != OrderStatus.PendingPayment)
                return Error.Conflict("order.payment_not_allowed");

            var pendingPayment = await _paymentRepository.GetAsync(p => p.OrderId == request.OrderId && p.Status == PaymentStatus.Pending, cancellationToken);
            if (pendingPayment != null)
            {
                var cancelResult = pendingPayment.MarkCancelled();
                if (cancelResult.IsFailure)
                    return cancelResult.Error;
            }

            var gateway = _paymentStrategies.FirstOrDefault(s => s.Provider == request.Provider);
            if (gateway == null)
                return Error.NotFound("payment.provider_not_supported");

            var sessionResult = await gateway.CreateSessionAsync(request.OrderId, order.FinalAmount, request.UserId, cancellationToken);
            if (sessionResult.IsFailure)
                return sessionResult.Errors[0];

            var session = sessionResult.Value;
            var paymentResult = Payment.Create(request.OrderId, order.FinalAmount, request.Provider, session.ProviderReference);
            if (paymentResult.IsFailure)
                return paymentResult.Error;

            await _paymentRepository.AddAsync(paymentResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new InitiatePaymentResponse(paymentResult.Value.Id, session.PaymentUrl);
        }
    }
}
