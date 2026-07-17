using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Announcements.Commands.SetAnnouncementPin
{
    public sealed class SetAnnouncementPinCommandValidator : AbstractValidator<SetAnnouncementPinCommand>
    {
        public SetAnnouncementPinCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.AnnouncementId)
                .ValidGuid("AnnouncementId");
        }
    }
}
