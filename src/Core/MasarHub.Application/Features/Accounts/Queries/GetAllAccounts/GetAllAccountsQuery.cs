using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Profiles;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Queries.GetAllAccounts
{
    public sealed record GetAllAccountsQuery
    (
        string? SearchTerm,
        UserRole? Role,
        bool? EmailConfirmed,
        bool? TwoFactorEnabled,
        bool? IsLocked,
        int PageNumber = 1,
        int PageSize = 10
    ) : IPaginatedQuery, IRequest<Result<PaginatedResult<AccountResponse>>>;
}
