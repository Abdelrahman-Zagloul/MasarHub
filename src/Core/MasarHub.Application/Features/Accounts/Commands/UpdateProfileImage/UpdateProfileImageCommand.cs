using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Commands.UpdateProfileImage
{
    public sealed record UpdateProfileImageCommand(Guid UserId, FileResource File) : IRequest<Result<string>>;
}
