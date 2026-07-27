using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Commands.ApproveInstructor
{
    public sealed record ApproveInstructorCommand(Guid InstructorUserId, Guid AdminUserId) : IRequest<Result>;
}
