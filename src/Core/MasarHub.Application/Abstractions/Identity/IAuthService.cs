using MasarHub.Application.Common.DI;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface IAuthService : IScopedService
    {
        Task<Result<string>> RegisterUserAsync(
            string fullName,
            string email,
            string password,
            string phoneNumber,
            Gender gender,
            UserRole role,
            CancellationToken ct = default);

        Task<Result<string>> GenerateEmailTokenAsync(Guid userId, CancellationToken ct = default);
    }
}
