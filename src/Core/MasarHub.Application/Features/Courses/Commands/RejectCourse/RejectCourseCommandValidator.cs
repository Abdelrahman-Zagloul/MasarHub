using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Courses.Commands.RejectCourse
{
    public sealed class RejectCourseCommandValidator : AbstractValidator<RejectCourseCommand>
    {
        public RejectCourseCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.Reason)
                .Required("Reason")
                .MinLengthValidation(5, "Reason")
                .MaxLengthValidation(1000, "Reason");
        }
    }
}