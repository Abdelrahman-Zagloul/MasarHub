using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor
{
    public sealed class GetCourseAnnouncementByIdForInstructorQueryValidator : AbstractValidator<GetCourseAnnouncementByIdForInstructorQuery>
    {
        public GetCourseAnnouncementByIdForInstructorQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.AnnouncementId)
                .ValidGuid("AnnouncementId");

            RuleFor(x => x.InstructorId)
                .ValidGuid("InstructorId");
        }
    }
}
