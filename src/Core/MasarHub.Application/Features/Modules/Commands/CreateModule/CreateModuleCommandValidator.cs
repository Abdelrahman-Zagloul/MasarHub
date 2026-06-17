using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Modules.Commands.CreateModule
{
    public sealed class CreateModuleCommandValidator : AbstractValidator<CreateModuleCommand>
    {
        public CreateModuleCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.Title)
                .Required("Title")
                .ValidMinLength(3, "Title")
                .ValidMaxLength(200, "Title");

            RuleFor(x => x.Description)
                .ValidMaxLength(2000, "Description");
        }
    }
}