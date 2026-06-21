using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Carts.Commands.AddToCart;
using MasarHub.Application.Features.Carts.Models;
using Moq;
using FluentAssertions;

namespace MasarHub.Application.UnitTests.Features.Carts.Commands.AddToCart
{
    [Trait("UnitTests.Feature.Carts", "AddToCart")]
    public sealed class AddToCartCommandHandlerTests
    {
        private readonly Mock<ICartService> _cartServiceMock;
        private readonly Mock<ICourseQuery> _courseQueryMock;
        private readonly AddToCartCommandHandler _sut;

        public AddToCartCommandHandlerTests()
        {
            _cartServiceMock = new Mock<ICartService>();
            _courseQueryMock = new Mock<ICourseQuery>();
            _sut = new AddToCartCommandHandler(_cartServiceMock.Object, _courseQueryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCourse_AddsItemAndReturnsSuccess()
        {
            var courseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new AddToCartCommand(courseId, userId);
            var courseData = new CourseCartData(courseId, "Test Course", 49.99m, null, true);

            _courseQueryMock
                .Setup(x => x.GetCourseCartDataAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courseData);

            _cartServiceMock
                .Setup(x => x.AddItemAsync(userId, It.IsAny<CartItem>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _cartServiceMock.Verify(x => x.AddItemAsync(userId, It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFound()
        {
            var command = new AddToCartCommand(Guid.NewGuid(), Guid.NewGuid());

            _courseQueryMock
                .Setup(x => x.GetCourseCartDataAsync(command.CourseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseCartData?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_found");
            _cartServiceMock.Verify(x => x.AddItemAsync(It.IsAny<Guid>(), It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CourseNotPublished_ReturnsBadRequest()
        {
            var courseId = Guid.NewGuid();
            var command = new AddToCartCommand(courseId, Guid.NewGuid());
            var courseData = new CourseCartData(courseId, "Test Course", 49.99m, null, false);

            _courseQueryMock
                .Setup(x => x.GetCourseCartDataAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courseData);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_published");
            _cartServiceMock.Verify(x => x.AddItemAsync(It.IsAny<Guid>(), It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CartServiceReturnsFailure_PropagatesError()
        {
            var courseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new AddToCartCommand(courseId, userId);
            var courseData = new CourseCartData(courseId, "Test Course", 49.99m, null, true);

            _courseQueryMock
                .Setup(x => x.GetCourseCartDataAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courseData);

            _cartServiceMock
                .Setup(x => x.AddItemAsync(userId, It.IsAny<CartItem>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.Conflict("cart.course_already_exists"));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "cart.course_already_exists");
        }
    }
}