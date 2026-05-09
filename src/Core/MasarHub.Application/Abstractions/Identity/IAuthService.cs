using MasarHub.Application.Common.DI;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.ConfirmEmail;
using MasarHub.Application.Features.Authentication.Commands.ResendConfirmEmail;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface IAuthService : IScopedService
    {
        Task<Result<RegisterUserResult>> RegisterUserAsync(
            string fullName,
            string email,
            string password,
            string phoneNumber,
            Gender gender,
            UserRole role,
            CancellationToken ct = default);

        Task<Result<ConfirmEmailTokenResult>> GenerateEmailTokenAsync(string email, CancellationToken ct = default);
        Task<Result<ConfirmedEmailResult>> ConfirmEmailAsync(string email, string token, CancellationToken ct = default);
        Task<Result> DeleteUserAsync(Guid userId, CancellationToken ct = default);
    }
}
