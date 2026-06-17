using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Modules.Commands.UpdateModule
{
    public sealed class UpdateModuleCommandValidator : AbstractValidator<UpdateModuleCommand>
    {
        public UpdateModuleCommandValidator()
        {
            RuleFor(x => x.ModuleId)
                .ValidGuid("ModuleId");

            RuleFor(x => x.Title)
                .ValidMinLength(3, "Title")
                .ValidMaxLength(200, "Title");

            RuleFor(x => x.Description)
                .ValidMaxLength(2000, "Description");
        }
    }
}
