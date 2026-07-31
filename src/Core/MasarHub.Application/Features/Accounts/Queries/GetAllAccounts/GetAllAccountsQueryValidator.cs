using MasarHub.Application.Common.Extensions;
using MasarHub.Application.Common.Pagination;

namespace MasarHub.Application.Features.Accounts.Queries.GetAllAccounts
{
    public sealed class GetAllAccountsQueryValidator : PaginationValidator<GetAllAccountsQuery>
    {
        public GetAllAccountsQueryValidator()
        {
            RuleFor(x => x.SearchTerm)
                .ValidMaxLength(100, "SearchTerm");

            RuleFor(x => x.Role)
                .ValidEnum("Role");
        }
    }
}
