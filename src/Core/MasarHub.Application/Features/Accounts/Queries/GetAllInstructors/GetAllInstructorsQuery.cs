using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Profiles;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Queries.GetAllInstructors
{
    public sealed record GetAllInstructorsQuery
    (
        string? SearchTerm,
        VerificationStatus? VerificationStatus,
        int PageNumber = 1,
        int PageSize = 10
    ) : IPaginatedQuery, IRequest<Result<PaginatedResult<InstructorAccountResponse>>>;
}
