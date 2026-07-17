using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Announcements.Commands.ScheduleCourseAnnouncement
{
    public sealed class ScheduleCourseAnnouncementCommandValidator : AbstractValidator<ScheduleCourseAnnouncementCommand>
    {
        public ScheduleCourseAnnouncementCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.AnnouncementId)
                .ValidGuid("AnnouncementId");

            RuleFor(x => x.ScheduledAt)
             .Must(scheduledAt => scheduledAt > DateTimeOffset.UtcNow)
             .WithErrorCode("course_announcement.invalid_schedule_time");
        }
    }
}
