using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCoursePrerequisites
{
    public sealed class UpdateCoursePrerequisitesCommandValidator : AbstractValidator<UpdateCoursePrerequisitesCommand>
    {
        public UpdateCoursePrerequisitesCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.Prerequisites)
               .RequiredCollection("Prerequisites");

            RuleForEach(x => x.Prerequisites)
                .Required("Prerequisite")
                .MaxLengthValidation(500, "Prerequisite");
        }
    }
}
