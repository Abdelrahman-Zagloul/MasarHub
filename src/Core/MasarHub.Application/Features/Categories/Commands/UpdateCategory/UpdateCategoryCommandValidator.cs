using FluentValidation;
using MasarHub.Application.Common.Extensions;
using MasarHub.Application.Features.Categories.Commands.UpdateCategory;

namespace MasarHub.Application.Features.Categories.Commands.UpdateCategoryName
{
    public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .ValidGuid("Id");

            RuleFor(x => x.Name)
                .MaxLengthValidation(200, "Name");

            RuleFor(x => x.ParentCategoryId)
                .ValidNullableGuid("ParentCategoryId");

            RuleFor(x => x)
                .Must(x =>
                    !string.IsNullOrWhiteSpace(x.Name)
                    || x.ParentCategoryId.HasValue
                    || x.MoveToRoot
                )
                .WithErrorCode("validation.at_least_one_field_required");

            RuleFor(x => x)
               .Must(x => !(x.MoveToRoot && x.ParentCategoryId.HasValue))
               .WithErrorCode("validation.cannot_use_parent_and_move_to_root");
        }
    }
}
