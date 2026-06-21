using FluentAssertions;
using MasarHub.Application.Features.Carts.Models;

namespace MasarHub.Application.UnitTests.Features.Carts.Models
{
    [Trait("UnitTests.Feature.Carts", "Cart")]
    public sealed class CartTests
    {
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly Guid CourseId = Guid.NewGuid();
        private static readonly CartItem ValidItem = CartItem.Create(CourseId, "Test Course", 99.99m, null);

        [Fact]
        public void Constructor_SetsUserIdAndLastModifiedAt()
        {
            var cart = new Cart(UserId);

            cart.UserId.Should().Be(UserId);
            cart.Items.Should().BeEmpty();
            cart.LastModifiedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void AddItem_ValidItem_AddsToItemsAndUpdatesLastModifiedAt()
        {
            var cart = new Cart(UserId);

            var result = cart.AddItem(ValidItem);

            result.IsSuccess.Should().BeTrue();
            cart.Items.Should().ContainSingle().Which.CourseId.Should().Be(CourseId);
            cart.LastModifiedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void AddItem_DuplicateCourseId_ReturnsConflict()
        {
            var cart = new Cart(UserId);
            cart.AddItem(ValidItem);

            var result = cart.AddItem(ValidItem);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "cart.course_already_exists");
            cart.Items.Should().HaveCount(1);
        }

        [Fact]
        public void RemoveItem_ExistingCourseId_RemovesItem()
        {
            var cart = new Cart(UserId);
            cart.AddItem(ValidItem);

            var result = cart.RemoveItem(CourseId);

            result.IsSuccess.Should().BeTrue();
            cart.Items.Should().BeEmpty();
        }

        [Fact]
        public void RemoveItem_NonExistingCourseId_ReturnsNotFound()
        {
            var cart = new Cart(UserId);

            var result = cart.RemoveItem(Guid.NewGuid());

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "cart.course_not_found");
        }
    }
}