using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Features.Carts.Commands.ClearCart;
using Moq;
using FluentAssertions;

namespace MasarHub.Application.UnitTests.Features.Carts.Commands.ClearCart
{
    [Trait("UnitTests.Feature.Carts", "ClearCart")]
    public sealed class ClearCartCommandHandlerTests
    {
        private readonly Mock<ICartService> _cartServiceMock;
        private readonly ClearCartCommandHandler _sut;

        public ClearCartCommandHandlerTests()
        {
            _cartServiceMock = new Mock<ICartService>();
            _sut = new ClearCartCommandHandler(_cartServiceMock.Object);
        }

        [Fact]
        public async Task Handle_CallsClearAsyncAndReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var command = new ClearCartCommand(userId);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _cartServiceMock.Verify(x => x.ClearAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}