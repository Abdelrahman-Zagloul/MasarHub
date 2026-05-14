using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Email.ConfirmEmail.Events
{
    public sealed record EmailConfirmedEvent(TokenUser User, string fullName) : INotification;
}
