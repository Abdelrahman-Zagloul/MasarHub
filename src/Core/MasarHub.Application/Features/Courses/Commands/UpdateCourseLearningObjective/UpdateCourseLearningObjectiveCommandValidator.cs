using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCourseLearningObjective
{
    public sealed class UpdateCourseLearningObjectiveCommandValidator : AbstractValidator<UpdateCourseLearningObjectiveCommand>
    {
        public UpdateCourseLearningObjectiveCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.LearningObjectives)
               .RequiredCollection("LearningObjectives");

            RuleForEach(x => x.LearningObjectives)
                .Required("LearningObjective")
                .ValidMaxLength(500, "LearningObjective");
        }
    }
}
