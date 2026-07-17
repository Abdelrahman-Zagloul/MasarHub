using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Announcements.Commands.PublishCourseAnnouncement
{
    public sealed class PublishCourseAnnouncementCommandValidator : AbstractValidator<PublishCourseAnnouncementCommand>
    {
        public PublishCourseAnnouncementCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.AnnouncementId)
                .ValidGuid("AnnouncementId");
        }
    }
}
