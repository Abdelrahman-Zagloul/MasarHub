using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Queries.GetAllAccounts
{
    public sealed class GetAllAccountsQueryHandler
        : IRequestHandler<GetAllAccountsQuery, Result<PaginatedResult<AccountResponse>>>
    {
        private readonly IAccountQuery _accountQuery;
        private readonly IFileStorageService _fileStorageService;

        public GetAllAccountsQueryHandler(IAccountQuery accountQuery, IFileStorageService fileStorageService)
        {
            _accountQuery = accountQuery;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<PaginatedResult<AccountResponse>>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _accountQuery.GetAllAsync(request, cancellationToken);

            foreach (var account in pagedResult.Items)
            {
                if (!string.IsNullOrWhiteSpace(account.ProfileImagePublicId))
                    account.ProfileImageUrl = _fileStorageService.GetUrl(account.ProfileImagePublicId, FileType.Image);
            }

            return PaginatedResult<AccountResponse>.Create(pagedResult.Items, pagedResult.TotalCount, request.PageNumber, request.PageSize);
        }
    }
}
