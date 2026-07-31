using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Queries.GetCurrentUser
{
    public sealed class GetCurrentUserQueryHandler
        : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>
    {
        private readonly IAccountQuery _accountQuery;
        private readonly ICurrentUserService _currentUser;
        private readonly IFileStorageService _fileStorageService;

        public GetCurrentUserQueryHandler(
            IAccountQuery accountQuery,
            ICurrentUserService currentUser,
            IFileStorageService fileStorageService)
        {
            _accountQuery = accountQuery;
            _currentUser = currentUser;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<CurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var data = _currentUser.IsInRole(Roles.Instructor)
                ? await _accountQuery.GetInstructorProfileAsync(_currentUser.UserId, cancellationToken)
                : await _accountQuery.GetUserProfileAsync(_currentUser.UserId, cancellationToken);

            if (data is null)
                return Error.NotFound("user.not_found");

            if (!string.IsNullOrEmpty(data.ProfileImagePublicId))
                data.ProfileImageUrl = _fileStorageService.GetUrl(data.ProfileImagePublicId, FileType.Image);

            return data;
        }
    }
}
