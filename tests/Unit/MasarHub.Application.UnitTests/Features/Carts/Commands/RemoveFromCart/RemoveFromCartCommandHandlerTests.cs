using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Carts.Commands.RemoveFromCart;
using Moq;
using FluentAssertions;
using MasarHub.Application.Common.Results.Errors;

namespace MasarHub.Application.UnitTests.Features.Carts.Commands.RemoveFromCart
{
    [Trait("UnitTests.Feature.Carts", "RemoveFromCart")]
    public sealed class RemoveFromCartCommandHandlerTests
    {
        private readonly Mock<ICartService> _cartServiceMock;
        private readonly RemoveFromCartCommandHandler _sut;

        public RemoveFromCartCommandHandlerTests()
        {
            _cartServiceMock = new Mock<ICartService>();
            _sut = new RemoveFromCartCommandHandler(_cartServiceMock.Object);
        }

        [Fact]
        public async Task Handle_CourseInCart_RemovesItemAndReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var command = new RemoveFromCartCommand(courseId, userId);

            _cartServiceMock
                .Setup(x => x.RemoveItemAsync(userId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _cartServiceMock.Verify(x => x.RemoveItemAsync(userId, courseId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CourseNotInCart_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var command = new RemoveFromCartCommand(courseId, userId);

            _cartServiceMock
                .Setup(x => x.RemoveItemAsync(userId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.NotFound("cart.course_not_found"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "cart.course_not_found");
        }
    }
}