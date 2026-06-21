using MasarHub.Application.Features.Carts.Models;
using FluentAssertions;

namespace MasarHub.Application.UnitTests.Features.Carts.Models
{
    [Trait("UnitTests.Feature.Carts", "CartItem")]
    public sealed class CartItemTests
    {
        private static readonly Guid CourseId = Guid.NewGuid();

        [Fact]
        public void Create_ValidInput_SetsAllProperties()
        {
            var item = CartItem.Create(CourseId, "Test Course", 49.99m, "thumb123");

            item.CourseId.Should().Be(CourseId);
            item.Title.Should().Be("Test Course");
            item.Price.Should().Be(49.99m);
            item.ThumbnailPublicId.Should().Be("thumb123");
            item.AddedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Create_NullThumbnail_SetsThumbnailToNull()
        {
            var item = CartItem.Create(CourseId, "Test Course", 0m, null);

            item.ThumbnailPublicId.Should().BeNull();
        }

        [Fact]
        public void Create_ZeroPrice_SetsPriceToZero()
        {
            var item = CartItem.Create(CourseId, "Free Course", 0m, null);

            item.Price.Should().Be(0m);
        }
    }
}