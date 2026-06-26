using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Payments.Commands.InitiatePayment;
using MasarHub.Domain.Modules.Orders;
using MasarHub.Domain.Modules.Payments;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Payments.Commands.InitiatePayment
{
    [Trait("UnitTests.Feature.Payments", "InitiatePayment")]
    public sealed class InitiatePaymentCommandHandlerTests
    {
        private readonly Mock<IRepository<Payment>> _paymentRepositoryMock;
        private readonly Mock<IOrderQuery> _orderQueryMock;
        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly InitiatePaymentCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly Guid OrderId = Guid.NewGuid();
        private static readonly PaymentProvider Provider = PaymentProvider.Stripe;

        public InitiatePaymentCommandHandlerTests()
        {
            _paymentRepositoryMock = new Mock<IRepository<Payment>>();
            _orderQueryMock = new Mock<IOrderQuery>();
            _paymentServiceMock = new Mock<IPaymentService>();
            _paymentServiceMock.Setup(x => x.Provider).Returns(Provider);
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _sut = new InitiatePaymentCommandHandler(
                _paymentRepositoryMock.Object,
                _orderQueryMock.Object,
                [_paymentServiceMock.Object],
                _unitOfWorkMock.Object);
        }

        private static OrderWithItems CreateOrderWithItems(OrderStatus status = OrderStatus.PendingPayment)
        {
            var order = Order.Create(UserId, "ORD-TEST-001").Value;
            typeof(Order).GetProperty(nameof(Order.Status))!.SetValue(order, status);
            order.AddItem(OrderItem.Create(Guid.NewGuid(), "Course 1", 100m, 0, null).Value);
            return new OrderWithItems(order, order.Items.ToList());
        }

        [Fact]
        public async Task Handle_OrderNotFound_ReturnsNotFound()
        {
            _orderQueryMock
                .Setup(x => x.GetOrderWithItemsAsync(OrderId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderWithItems?)null);

            var result = await _sut.Handle(new InitiatePaymentCommand(UserId, OrderId, Provider), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "order.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_OrderNotPendingPayment_ReturnsConflict()
        {
            var orderWithItems = CreateOrderWithItems(OrderStatus.Paid);

            _orderQueryMock
                .Setup(x => x.GetOrderWithItemsAsync(orderWithItems.Order.Id, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderWithItems);

            var result = await _sut.Handle(new InitiatePaymentCommand(UserId, orderWithItems.Order.Id, Provider), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "order.payment_not_allowed");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExistingPendingPaymentIsCancelled()
        {
            var orderWithItems = CreateOrderWithItems();
            var existingPayment = Payment.Create(orderWithItems.Order.Id, 100m, Provider, "existing_ref").Value;

            _orderQueryMock
                .Setup(x => x.GetOrderWithItemsAsync(orderWithItems.Order.Id, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderWithItems);

            _paymentRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, object>>[]>()))
                .ReturnsAsync(existingPayment);

            _paymentServiceMock
                .Setup(x => x.CreateSessionAsync(orderWithItems.Order, orderWithItems.Items, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentCreationResult("sess_123", "https://checkout.url"));

            var result = await _sut.Handle(new InitiatePaymentCommand(UserId, orderWithItems.Order.Id, Provider), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            existingPayment.Status.Should().Be(PaymentStatus.Cancelled);
            _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ProviderNotSupported_ReturnsNotFound()
        {
            var orderWithItems = CreateOrderWithItems();

            _orderQueryMock
                .Setup(x => x.GetOrderWithItemsAsync(orderWithItems.Order.Id, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderWithItems);

            var command = new InitiatePaymentCommand(UserId, orderWithItems.Order.Id, PaymentProvider.Mock);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "payment.provider_not_supported");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_SessionCreationFails_ReturnsError()
        {
            var orderWithItems = CreateOrderWithItems();

            _orderQueryMock
                .Setup(x => x.GetOrderWithItemsAsync(orderWithItems.Order.Id, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderWithItems);

            _paymentServiceMock
                .Setup(x => x.CreateSessionAsync(orderWithItems.Order, orderWithItems.Items, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<PaymentCreationResult>.Failure(Error.Failure("stripe.session_creation_failed")));

            var result = await _sut.Handle(new InitiatePaymentCommand(UserId, orderWithItems.Order.Id, Provider), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "stripe.session_creation_failed");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccess()
        {
            var orderWithItems = CreateOrderWithItems();
            var paymentUrl = "https://checkout.stripe.com/session_123";

            _orderQueryMock
                .Setup(x => x.GetOrderWithItemsAsync(orderWithItems.Order.Id, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderWithItems);

            _paymentServiceMock
                .Setup(x => x.CreateSessionAsync(orderWithItems.Order, orderWithItems.Items, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentCreationResult("sess_123", paymentUrl));

            var result = await _sut.Handle(new InitiatePaymentCommand(UserId, orderWithItems.Order.Id, Provider), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.PaymentUrl.Should().Be(paymentUrl);
            result.Value.PaymentId.Should().NotBeEmpty();
            _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
