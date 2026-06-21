using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Carts.Commands.RemoveFromCart
{
    public sealed class RemoveFromCartCommandValidator : AbstractValidator<RemoveFromCartCommand>
    {
        public RemoveFromCartCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");
        }
    }
}