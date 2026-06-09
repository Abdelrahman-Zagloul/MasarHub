using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Modules.Commands.UpdateModule
{
    public class UpdateModuleCommandValidator : AbstractValidator<UpdateModuleCommand>
    {
        public UpdateModuleCommandValidator()
        {
            RuleFor(x => x.ModuleId)
                .ValidGuid("ModuleId");

            RuleFor(x => x.Title)
                .MinLengthValidation(3, "Title")
                .MaxLengthValidation(200, "Title");

            RuleFor(x => x.Description)
                .MaxLengthValidation(2000, "Description");
        }
    }
}
