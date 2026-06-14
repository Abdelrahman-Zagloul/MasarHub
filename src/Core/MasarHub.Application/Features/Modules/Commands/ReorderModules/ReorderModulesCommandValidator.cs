using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Modules.Commands.ReorderModules
{
    public sealed class ReorderModulesCommandValidator : AbstractValidator<ReorderModulesCommand>
    {
        public ReorderModulesCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.OrderedModuleIds)
                .Cascade(CascadeMode.Stop)
                .RequiredNonEmptyCollection("OrderedModuleIds")

                .Must(ids => ids.Count == ids.ToHashSet().Count)
                .WithErrorCode("module.duplicate_reorder_module_ids")

                .Must(ids => ids.All(id => id != Guid.Empty))
                .WithErrorCode("module.invalid_module_ids");
        }
    }
}
