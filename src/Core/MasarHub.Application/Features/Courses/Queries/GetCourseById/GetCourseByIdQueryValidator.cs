using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Courses.Queries.GetCourseById
{
    public sealed class GetCourseByIdQueryValidator : AbstractValidator<GetCourseByIdQuery>
    {
        public GetCourseByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .ValidGuid("CourseId");
        }
    }
}
