using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Accounts.Commands.ApproveInstructor
{
    public sealed class ApproveInstructorCommandValidator : AbstractValidator<ApproveInstructorCommand>
    {
        public ApproveInstructorCommandValidator()
        {
            RuleFor(x => x.InstructorUserId)
                .ValidGuid("InstructorId");
        }
    }
}
