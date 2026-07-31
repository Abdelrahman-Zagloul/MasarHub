using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Queries.GetCurrentUser
{
    public sealed record GetCurrentUserQuery : IRequest<Result<CurrentUserResponse>>;
}
