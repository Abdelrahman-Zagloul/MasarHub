using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Announcements.Commands.UpdateCourseAnnouncement
{
    public sealed class UpdateCourseAnnouncementCommandValidator : AbstractValidator<UpdateCourseAnnouncementCommand>
    {
        public UpdateCourseAnnouncementCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.AnnouncementId)
                .ValidGuid("AnnouncementId");

            RuleFor(x => x.Title)
                .ValidMinLength(5, "Title")
                .ValidMaxLength(200, "Title");

            RuleFor(x => x.Content)
                .ValidMinLength(5, "Content")
                .ValidMaxLength(4000, "Content");

            RuleFor(x => x.Importance)
                .ValidEnum("Importance");

            RuleFor(x => x)
                .Must(x => x.Title != null || x.Content != null || x.Importance != null)
                .WithErrorCode("validation.at_least_one");
        }
    }
}
