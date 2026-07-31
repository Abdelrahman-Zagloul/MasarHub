using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Accounts.Queries.GetCurrentUser;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Queries.GetAccountById
{
    public sealed record GetAccountByIdQuery(Guid UserId) : IRequest<Result<CurrentUserResponse>>;
}
