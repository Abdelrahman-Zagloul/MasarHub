using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Profiles;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.RegisterStudent
{
    public sealed record RegisterStudentCommand
    (
        string FullName,
        string Email,
        string PhoneNumber,
        Gender Gender,
        string Password
    ) : IRequest<Result>;
}
