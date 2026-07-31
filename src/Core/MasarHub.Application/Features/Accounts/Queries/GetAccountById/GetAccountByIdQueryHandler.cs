using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Accounts.Queries.GetCurrentUser;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Queries.GetAccountById
{
    public sealed class GetAccountByIdQueryHandler
        : IRequestHandler<GetAccountByIdQuery, Result<CurrentUserResponse>>
    {
        private readonly IAccountQuery _accountQuery;
        private readonly IFileStorageService _fileStorageService;

        public GetAccountByIdQueryHandler(IAccountQuery accountQuery, IFileStorageService fileStorageService)
        {
            _accountQuery = accountQuery;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<CurrentUserResponse>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountQuery.GetAccountByIdAsync(request.UserId, cancellationToken);

            if (account is null)
                return Error.NotFound("user.not_found");

            if (!string.IsNullOrWhiteSpace(account.ProfileImagePublicId))
                account.ProfileImageUrl = _fileStorageService.GetUrl(account.ProfileImagePublicId, FileType.Image);

            return account;
        }
    }
}
