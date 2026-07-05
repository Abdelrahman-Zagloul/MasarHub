using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Progress.Queries.GetCourseProgress
{
    public sealed class GetCourseProgressQueryValidator : AbstractValidator<GetCourseProgressQuery>
    {
        public GetCourseProgressQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");
        }
    }
}
