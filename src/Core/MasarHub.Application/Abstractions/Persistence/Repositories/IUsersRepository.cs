using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Persistence.Repositories
{
    public interface IUsersRepository : IScopedService
    {
        Task<Result> UpdateProfileImageAsync(Guid userId, string newPublicId, CancellationToken ct = default);
        Task<Result> UpdateAccountAsync(Guid userId, string? phoneNumber, Gender? gender, TwoFactorProvider? preferredTwoFactorProvider, CancellationToken ct = default);
    }
}
