using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Accounts.Queries.GetAllAccounts;
using MasarHub.Application.Features.Accounts.Queries.GetCurrentUser;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface IAccountQuery : IScopedService
    {
        Task<VerificationStatus?> GetInstructorStatusAsync(Guid userId, CancellationToken ct = default);
        Task<CurrentUserResponse?> GetUserProfileAsync(Guid userId, CancellationToken ct = default);
        Task<CurrentUserResponse?> GetInstructorProfileAsync(Guid userId, CancellationToken ct = default);
        Task<PagedResult<AccountResponse>> GetAllAsync(GetAllAccountsQuery query, CancellationToken ct = default);
    }
}
