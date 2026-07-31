using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Queries.GetAllInstructors
{
    public sealed class GetAllInstructorsQueryHandler
        : IRequestHandler<GetAllInstructorsQuery, Result<PaginatedResult<InstructorAccountResponse>>>
    {
        private readonly IAccountQuery _accountQuery;
        private readonly IFileStorageService _fileStorageService;

        public GetAllInstructorsQueryHandler(IAccountQuery accountQuery, IFileStorageService fileStorageService)
        {
            _accountQuery = accountQuery;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<PaginatedResult<InstructorAccountResponse>>> Handle(GetAllInstructorsQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _accountQuery.GetAllInstructorsAsync(request, cancellationToken);

            foreach (var instructor in pagedResult.Items)
            {
                if (!string.IsNullOrWhiteSpace(instructor.ProfileImagePublicId))
                    instructor.ProfileImageUrl = _fileStorageService.GetUrl(instructor.ProfileImagePublicId, FileType.Image);
            }

            return PaginatedResult<InstructorAccountResponse>.Create(pagedResult.Items, pagedResult.TotalCount, request.PageNumber, request.PageSize);
        }
    }
}
