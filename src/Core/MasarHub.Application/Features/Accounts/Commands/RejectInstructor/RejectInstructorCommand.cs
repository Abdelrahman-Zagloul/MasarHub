using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Commands.RejectInstructor
{
    public sealed record RejectInstructorCommand(Guid InstructorUserId, Guid AdminUserId, string Reason) : IRequest<Result>;
    public sealed record RejectInstructorRequest(string Reason);
}
