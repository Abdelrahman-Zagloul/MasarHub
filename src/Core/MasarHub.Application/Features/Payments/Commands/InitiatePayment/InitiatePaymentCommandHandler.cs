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
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public InitiatePaymentCommandHandler(IRepository<Payment> paymentRepository, IOrderQuery orderQuery, IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _orderQuery = orderQuery;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<InitiatePaymentResponse>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {
            var data = await _orderQuery.GetOrderInitiateDataAsync(request.OrderId, cancellationToken);
            if (data.Order == null)
                return Error.NotFound("order.not_found");

            if (data.Order.UserId != request.UserId)
                return Error.Forbidden("order.not_owned");

            if (data.Order.Status != OrderStatus.PendingPayment)
                Error.Conflict("order.payment_not_allowed");

            var sessionResult = await _paymentService.CreateSessionAsync(request.OrderId, data.Order.FinalAmount, request.UserId, cancellationToken);
            if (sessionResult.IsFailure)
                return sessionResult.Errors[0];

            var session = sessionResult.Value;
            if (data.HasExistingPayment)
            {
                var existingPayment = await _paymentRepository.GetAsync(p => p.OrderId == request.OrderId, cancellationToken);
                if (existingPayment == null)
                    return Error.NotFound("payment.not_found");

                existingPayment.UpdateProviderReference(session.ProviderReference);

                _paymentRepository.Update(existingPayment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return new InitiatePaymentResponse(existingPayment.Id, session.PaymentUrl);
            }

            var paymentResult = Payment.Create(request.OrderId, data.Order.FinalAmount, request.Provider, session.ProviderReference);
            if (paymentResult.IsFailure)
                return paymentResult.Error;

            await _paymentRepository.AddAsync(paymentResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new InitiatePaymentResponse(paymentResult.Value.Id, session.PaymentUrl);
        }
    }
}
