using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Orders.Commands.CancelOrder;
using MasarHub.Domain.Modules.Orders;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Orders.Commands.CancelOrder
{
    [Trait("UnitTests.Feature.Orders", "CancelOrder")]
    public sealed class CancelOrderCommandHandlerTests
    {
        private readonly Mock<IRepository<Order>> _orderRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CancelOrderCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly Guid OrderId = Guid.NewGuid();

        public CancelOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IRepository<Order>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new CancelOrderCommandHandler(_orderRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        private Order CreatePendingOrder(Guid? userId = null)
        {
            var order = Order.Create(userId ?? UserId, "ORD-TEST-12345678").Value;
            order.AddItem(OrderItem.Create(Guid.NewGuid(), "Course 1", 100m, 0, null).Value);
            return order;
        }

        [Fact]
        public async Task Handle_OrderNotFound_ReturnsNotFound()
        {
            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(OrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            var result = await _sut.Handle(new CancelOrderCommand(UserId, OrderId), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == OrderErrors.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_OrderNotOwnedByUser_ReturnsConflict()
        {
            var order = CreatePendingOrder(userId: Guid.NewGuid());

            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var result = await _sut.Handle(new CancelOrderCommand(UserId, order.Id), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == OrderErrors.NotOwned);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AlreadyCancelledOrder_ReturnsInvalidStatusTransition()
        {
            var order = CreatePendingOrder();
            order.Cancel();

            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var result = await _sut.Handle(new CancelOrderCommand(UserId, order.Id), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == OrderErrors.InvalidStatusTransition.Code);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidCancel_ReturnsSuccess()
        {
            var order = CreatePendingOrder();

            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var result = await _sut.Handle(new CancelOrderCommand(UserId, order.Id), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Cancelled);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
