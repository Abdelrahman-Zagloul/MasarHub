using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCourseRequirements
{
    public sealed class UpdateCourseRequirementsCommandValidator : AbstractValidator<UpdateCourseRequirementsCommand>
    {
        public UpdateCourseRequirementsCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.Requirements)
               .RequiredCollection("Requirements");

            RuleForEach(x => x.Requirements)
                .Required("Requirement")
                .ValidMaxLength(500, "Requirement");
        }
    }
}