using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Orders;
using MediatR;

namespace MasarHub.Application.Features.Orders.Commands.CancelOrder
{
    public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelOrderCommandHandler(IRepository<Order> orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Error.NotFound(OrderErrors.NotFound);

            if (order.UserId != request.UserId)
                return Error.Conflict(OrderErrors.NotOwned);

            var cancelResult = order.Cancel();
            if (cancelResult.IsFailure)
                return cancelResult.Error;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
