using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent
{
    public sealed class GetCourseAnnouncementByIdForStudentQueryValidator : AbstractValidator<GetCourseAnnouncementByIdForStudentQuery>
    {
        public GetCourseAnnouncementByIdForStudentQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.AnnouncementId)
                .ValidGuid("AnnouncementId");

            RuleFor(x => x.StudentId)
                .ValidGuid("StudentId");
        }
    }
}
