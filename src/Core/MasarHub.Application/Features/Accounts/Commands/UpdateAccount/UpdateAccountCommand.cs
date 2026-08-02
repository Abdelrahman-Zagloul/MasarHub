using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Profiles;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Commands.UpdateAccount
{
    public sealed record UpdateAccountCommand
    (
        Guid UserId,
        string? PhoneNumber,
        Gender? Gender,
        TwoFactorProvider? PreferredTwoFactorProvider
    ) : IRequest<Result>;

    public sealed record UpdateAccountRequest
    (
        string? PhoneNumber,
        Gender? Gender,
        TwoFactorProvider? PreferredTwoFactorProvider
    );
}
