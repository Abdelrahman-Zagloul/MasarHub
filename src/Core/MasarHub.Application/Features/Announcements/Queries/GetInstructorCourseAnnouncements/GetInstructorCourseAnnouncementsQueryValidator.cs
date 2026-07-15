using FluentValidation;
using MasarHub.Application.Common.Extensions;
using MasarHub.Application.Common.Pagination;

namespace MasarHub.Application.Features.Announcements.Queries.GetInstructorCourseAnnouncements
{
    public sealed class GetInstructorCourseAnnouncementsQueryValidator : PaginationValidator<GetInstructorCourseAnnouncementsQuery>
    {
        public GetInstructorCourseAnnouncementsQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.InstructorId)
                .ValidGuid("InstructorId");

            RuleFor(x => x.Importance)
                .ValidEnum("Importance");

            RuleFor(x => x.Search)
                .ValidMaxLength(200, "Search");
        }
    }
}
