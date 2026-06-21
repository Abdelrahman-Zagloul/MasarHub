using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Features.Carts.Models;
using MasarHub.Application.Features.Carts.Queries.GetCart;
using Moq;
using FluentAssertions;

namespace MasarHub.Application.UnitTests.Features.Carts.Queries.GetCart
{
    [Trait("UnitTests.Feature.Carts", "GetCart")]
    public sealed class GetCartQueryHandlerTests
    {
        private readonly Mock<ICartService> _cartServiceMock;
        private readonly GetCartQueryHandler _sut;

        public GetCartQueryHandlerTests()
        {
            _cartServiceMock = new Mock<ICartService>();
            _sut = new GetCartQueryHandler(_cartServiceMock.Object);
        }

        [Fact]
        public async Task Handle_EmptyCart_ReturnsEmptyResponse()
        {
            var userId = Guid.NewGuid();
            var query = new GetCartQuery(userId);
            var cart = new Cart(userId);

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().BeEmpty();
            result.Value.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task Handle_CartWithItems_ReturnsMappedResponse()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var query = new GetCartQuery(userId);
            var cart = new Cart(userId);
            cart.AddItem(CartItem.Create(courseId, "Test Course", 49.99m, "thumb"));

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
            result.Value.TotalCount.Should().Be(1);

            var item = result.Value.Items[0];
            item.CourseId.Should().Be(courseId);
            item.Title.Should().Be("Test Course");
            item.Price.Should().Be(49.99m);
            item.ThumbnailPublicId.Should().Be("thumb");
        }

        [Fact]
        public async Task Handle_MultipleItems_ReturnsCorrectTotalItems()
        {
            var userId = Guid.NewGuid();
            var query = new GetCartQuery(userId);
            var cart = new Cart(userId);
            cart.AddItem(CartItem.Create(Guid.NewGuid(), "Course 1", 10m, null));
            cart.AddItem(CartItem.Create(Guid.NewGuid(), "Course 2", 20m, null));
            cart.AddItem(CartItem.Create(Guid.NewGuid(), "Course 3", 30m, null));

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(3);
            result.Value.TotalCount.Should().Be(3);
        }
    }
}