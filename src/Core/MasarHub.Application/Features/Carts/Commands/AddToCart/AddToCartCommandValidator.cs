using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Carts.Commands.AddToCart
{
    public sealed class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCartCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");
        }
    }
}