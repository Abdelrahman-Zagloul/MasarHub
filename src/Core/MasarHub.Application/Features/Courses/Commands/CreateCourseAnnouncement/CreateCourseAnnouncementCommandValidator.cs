using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Courses.Commands.CreateCourseAnnouncement
{
    public sealed class CreateCourseAnnouncementCommandValidator : AbstractValidator<CreateCourseAnnouncementCommand>
    {
        public CreateCourseAnnouncementCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.Title)
                .Required("Title")
                .ValidMinLength(5, "Title")
                .ValidMaxLength(200, "Title");

            RuleFor(x => x.Content)
                .Required("Content")
                .ValidMinLength(5, "Content")
                .ValidMaxLength(4000, "Content");

            RuleFor(x => x.Importance)
                .ValidEnum("Importance");
        }
    }
}
