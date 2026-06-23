using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Orders.Commands.DeleteOrder;
using MasarHub.Domain.Modules.Orders;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Orders.Commands.DeleteOrder
{
    [Trait("UnitTests.Feature.Orders", "DeleteOrder")]
    public sealed class DeleteOrderCommandHandlerTests
    {
        private readonly Mock<IRepository<Order>> _orderRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteOrderCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly Guid OrderId = Guid.NewGuid();

        public DeleteOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IRepository<Order>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new DeleteOrderCommandHandler(_orderRepositoryMock.Object, _unitOfWorkMock.Object);
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

            var result = await _sut.Handle(new DeleteOrderCommand(UserId, OrderId), CancellationToken.None);

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

            var result = await _sut.Handle(new DeleteOrderCommand(UserId, order.Id), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == OrderErrors.NotOwned);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_PaidOrder_ReturnsInvalidStatusForDeletion()
        {
            var order = CreatePendingOrder();
            order.MarkPaid();

            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var result = await _sut.Handle(new DeleteOrderCommand(UserId, order.Id), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == OrderErrors.InvalidStatusForDeletion.Code);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_FailedOrder_ReturnsInvalidStatusForDeletion()
        {
            var order = CreatePendingOrder();
            order.MarkFailed();

            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var result = await _sut.Handle(new DeleteOrderCommand(UserId, order.Id), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == OrderErrors.InvalidStatusForDeletion.Code);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CancelledOrder_CanDelete()
        {
            var order = CreatePendingOrder();
            order.Cancel();

            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var result = await _sut.Handle(new DeleteOrderCommand(UserId, order.Id), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            order.IsDeleted.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AlreadyDeletedOrder_ReturnsSuccess()
        {
            var order = CreatePendingOrder();
            order.Delete();

            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var result = await _sut.Handle(new DeleteOrderCommand(UserId, order.Id), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidDelete_ReturnsSuccess()
        {
            var order = CreatePendingOrder();

            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var result = await _sut.Handle(new DeleteOrderCommand(UserId, order.Id), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            order.IsDeleted.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
