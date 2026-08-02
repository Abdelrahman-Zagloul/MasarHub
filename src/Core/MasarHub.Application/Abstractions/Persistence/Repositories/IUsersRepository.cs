using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Results;

namespace MasarHub.Application.Abstractions.Persistence.Repositories
{
    public interface IUsersRepository : IScopedService
    {
        Task<Result> UpdateProfileImageAsync(Guid userId, string newPublicId, CancellationToken ct = default);
    }
}
