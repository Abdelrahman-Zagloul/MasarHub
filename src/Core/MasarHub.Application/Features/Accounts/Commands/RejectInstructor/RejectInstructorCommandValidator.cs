using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Accounts.Commands.RejectInstructor
{
    public sealed class RejectInstructorCommandValidator : AbstractValidator<RejectInstructorCommand>
    {
        public RejectInstructorCommandValidator()
        {
            RuleFor(x => x.InstructorUserId)
                .ValidGuid("InstructorId");

            RuleFor(x => x.Reason)
                .Required("Reason")
                .ValidMinLength(5, "Reason")
                .ValidMaxLength(500, "Reason");
        }
    }
}
