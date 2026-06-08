using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Modules.Commands.CreateModule
{
    public class CreateModuleCommandValidator : AbstractValidator<CreateModuleCommand>
    {
        public CreateModuleCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.Title)
                .Required("Title")
                .MinLengthValidation(3, "Title")
                .MaxLengthValidation(200, "Title");

            RuleFor(x => x.Description)
                .MaxLengthValidation(2000, "Description");
        }
    }
}