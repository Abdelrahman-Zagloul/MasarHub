using FluentValidation;
using MasarHub.Application.Common.Extensions;
using MasarHub.Application.Common.Pagination;

namespace MasarHub.Application.Features.Announcements.Queries.GetStudentCourseAnnouncements
{
    public sealed class GetStudentCourseAnnouncementsQueryValidator : PaginationValidator<GetStudentCourseAnnouncementsQuery>
    {
        public GetStudentCourseAnnouncementsQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.StudentId)
                .ValidGuid("StudentId");

            RuleFor(x => x.Importance)
                .ValidEnum("Importance");

            RuleFor(x => x.Search)
                .ValidMaxLength(200, "Search");
        }
    }
}
