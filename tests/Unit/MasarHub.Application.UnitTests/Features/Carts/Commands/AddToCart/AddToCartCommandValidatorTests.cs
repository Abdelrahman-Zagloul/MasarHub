using MasarHub.Application.Features.Carts.Commands.AddToCart;
using FluentAssertions;

namespace MasarHub.Application.UnitTests.Features.Carts.Commands.AddToCart
{
    [Trait("UnitTests.Feature.Carts", "AddToCart")]
    public sealed class AddToCartCommandValidatorTests
    {
        private readonly AddToCartCommandValidator _sut = new();

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var command = new AddToCartCommand(Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new AddToCartCommand(Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}