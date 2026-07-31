using MasarHub.Application.Common.Extensions;
using MasarHub.Application.Common.Pagination;

namespace MasarHub.Application.Features.Accounts.Queries.GetAllInstructors
{
    public sealed class GetAllInstructorsQueryValidator : PaginationValidator<GetAllInstructorsQuery>
    {
        public GetAllInstructorsQueryValidator()
        {
            RuleFor(x => x.SearchTerm)
                .ValidMaxLength(100, "SearchTerm");

            RuleFor(x => x.VerificationStatus)
                .ValidEnum("VerificationStatus");
        }
    }
}
