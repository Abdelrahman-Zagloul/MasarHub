using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Categories.Commands.UpdateCategoryName
{
    public sealed class UpdateCategoryNameCommandValidator : AbstractValidator<UpdateCategoryNameCommand>
    {
        public UpdateCategoryNameCommandValidator()
        {
            RuleFor(x => x.Id)
                .ValidGuid("Id");

            RuleFor(x => x.Name)
                .Required("Name")
                .MaxLengthValidation(200, "Name");
        }
    }
}
