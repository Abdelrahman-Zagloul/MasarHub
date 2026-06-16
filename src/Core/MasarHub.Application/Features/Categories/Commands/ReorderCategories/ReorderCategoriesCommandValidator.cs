using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Categories.Commands.ReorderCategories
{
    public sealed class ReorderCategoriesCommandValidator : AbstractValidator<ReorderCategoriesCommand>
    {
        public ReorderCategoriesCommandValidator()
        {
            RuleFor(x => x.ParentCategoryId)
                .ValidGuid("ParentCategoryId");

            RuleFor(x => x.OrderedCategoryIds)
                .Cascade(CascadeMode.Stop)
                .RequiredNonEmptyCollection("OrderedCategoryIds")

                .Must(ids => ids.Count == ids.ToHashSet().Count)
                .WithErrorCode("category.duplicate_reorder_category_ids")

                .Must(ids => ids.All(id => id != Guid.Empty))
                .WithErrorCode("category.invalid_category_ids");
        }
    }
}