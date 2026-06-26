using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Payments.Commands.PaymentWebhook;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Orders;
using MasarHub.Domain.Modules.Payments;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Payments.Commands.PaymentWebhook
{
    [Trait("UnitTests.Feature.Payments", "PaymentWebhook")]
    public sealed class PaymentWebhookCommandHandlerTests
    {
        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly Mock<IRepository<Payment>> _paymentRepositoryMock;
        private readonly Mock<IRepository<Order>> _orderRepositoryMock;
        private readonly Mock<IRepository<CourseEnrollment>> _enrollmentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly PaymentWebhookCommandHandler _sut;
        private static readonly PaymentProvider Provider = PaymentProvider.Stripe;
        private const string ProviderReference = "cs_test_123";
        private const string RawBody = "{\"test\":true}";
        private static readonly Dictionary<string, string> Headers = new() { { "Stripe-Signature", "sig123" } };

        public PaymentWebhookCommandHandlerTests()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _paymentServiceMock.Setup(x => x.Provider).Returns(Provider);
            _paymentRepositoryMock = new Mock<IRepository<Payment>>();
            _orderRepositoryMock = new Mock<IRepository<Order>>();
            _enrollmentRepositoryMock = new Mock<IRepository<CourseEnrollment>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _sut = new PaymentWebhookCommandHandler(
                [_paymentServiceMock.Object],
                _paymentRepositoryMock.Object,
                _orderRepositoryMock.Object,
                _enrollmentRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        private static Payment CreatePendingPayment()
        {
            return Payment.Create(Guid.NewGuid(), 100m, Provider, ProviderReference).Value;
        }

        private static Order CreatePendingOrderWithItem()
        {
            var order = Order.Create(Guid.NewGuid(), "ORD-TEST-001").Value;
            order.AddItem(OrderItem.Create(Guid.NewGuid(), "Course 1", 100m, 0, null).Value);
            typeof(Order).GetProperty(nameof(Order.Status))!.SetValue(order, OrderStatus.PendingPayment);
            return order;
        }

        [Fact]
        public async Task Handle_ProviderNotSupported_ReturnsNotFound()
        {
            var handler = new PaymentWebhookCommandHandler([], _paymentRepositoryMock.Object, _orderRepositoryMock.Object, _enrollmentRepositoryMock.Object, _unitOfWorkMock.Object);

            var result = await handler.Handle(new PaymentWebhookCommand(Provider, RawBody, Headers), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "payment.provider_not_supported");
        }

        [Fact]
        public async Task Handle_WebhookValidationFails_ReturnsError()
        {
            _paymentServiceMock
                .Setup(x => x.ValidateWebhookAsync(RawBody, Headers, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<PaymentWebhookValidationResult>.Failure(Error.Failure("stripe.webhook_validation_failed")));

            var result = await _sut.Handle(new PaymentWebhookCommand(Provider, RawBody, Headers), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "stripe.webhook_validation_failed");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_PaymentNotFound_ReturnsNotFound()
        {
            _paymentServiceMock
                .Setup(x => x.ValidateWebhookAsync(RawBody, Headers, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentWebhookValidationResult(ProviderReference, PaymentStatus.Succeeded));

            _paymentRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, object>>[]>()))
                .ReturnsAsync((Payment?)null);

            var result = await _sut.Handle(new PaymentWebhookCommand(Provider, RawBody, Headers), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "payment.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_PaymentIsPending_ReturnsEarly()
        {
            var payment = CreatePendingPayment();

            _paymentServiceMock
                .Setup(x => x.ValidateWebhookAsync(RawBody, Headers, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentWebhookValidationResult(ProviderReference, PaymentStatus.Pending));

            _paymentRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, object>>[]>()))
                .ReturnsAsync(payment);

            var result = await _sut.Handle(new PaymentWebhookCommand(Provider, RawBody, Headers), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Status.Should().Be(PaymentStatus.Pending);
            payment.Status.Should().Be(PaymentStatus.Pending);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_PaymentSucceeded_MarksPaymentAndOrderAndCreatesEnrollments()
        {
            var payment = CreatePendingPayment();
            var order = CreatePendingOrderWithItem();

            _paymentServiceMock
                .Setup(x => x.ValidateWebhookAsync(RawBody, Headers, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentWebhookValidationResult(ProviderReference, PaymentStatus.Succeeded));

            _paymentRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, object>>[]>()))
                .ReturnsAsync(payment);

            _orderRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Order, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<Order, object>>[]>()))
                .ReturnsAsync(order);

            var result = await _sut.Handle(new PaymentWebhookCommand(Provider, RawBody, Headers), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Status.Should().Be(PaymentStatus.Succeeded);
            payment.Status.Should().Be(PaymentStatus.Succeeded);
            order.Status.Should().Be(OrderStatus.Paid);
            _enrollmentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CourseEnrollment>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PaymentAlreadySucceeded_ReturnsSuccessIdempotent()
        {
            var payment = CreatePendingPayment();
            payment.MarkSucceeded(Guid.NewGuid(), "ORD-TEST-001");
            var order = CreatePendingOrderWithItem();

            _paymentServiceMock
                .Setup(x => x.ValidateWebhookAsync(RawBody, Headers, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentWebhookValidationResult(ProviderReference, PaymentStatus.Succeeded));

            _paymentRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, object>>[]>()))
                .ReturnsAsync(payment);

            _orderRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Order, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<Order, object>>[]>()))
                .ReturnsAsync(order);

            var result = await _sut.Handle(new PaymentWebhookCommand(Provider, RawBody, Headers), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Status.Should().Be(PaymentStatus.Succeeded);
            _enrollmentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CourseEnrollment>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PaymentExpired_MarksPaymentExpired()
        {
            var payment = CreatePendingPayment();

            _paymentServiceMock
                .Setup(x => x.ValidateWebhookAsync(RawBody, Headers, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentWebhookValidationResult(ProviderReference, PaymentStatus.Expired));

            _paymentRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, object>>[]>()))
                .ReturnsAsync(payment);

            var result = await _sut.Handle(new PaymentWebhookCommand(Provider, RawBody, Headers), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Status.Should().Be(PaymentStatus.Expired);
            payment.Status.Should().Be(PaymentStatus.Expired);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PaymentFailed_MarksPaymentFailed()
        {
            var payment = CreatePendingPayment();
            var order = Order.Create(Guid.NewGuid(), "ORD-TEST-001").Value;

            _paymentServiceMock
                .Setup(x => x.ValidateWebhookAsync(RawBody, Headers, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentWebhookValidationResult(ProviderReference, PaymentStatus.Failed));

            _paymentRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<Payment, object>>[]>()))
                .ReturnsAsync(payment);

            _orderRepositoryMock
                .Setup(x => x.GetByIdAsync(payment.OrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var result = await _sut.Handle(new PaymentWebhookCommand(Provider, RawBody, Headers), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Status.Should().Be(PaymentStatus.Failed);
            payment.Status.Should().Be(PaymentStatus.Failed);
            order.Status.Should().Be(OrderStatus.PendingPayment);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
