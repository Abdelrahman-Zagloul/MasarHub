using MasarHub.Application.Features.Authentication.Commands.Email.ResendConfirmEmail;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Email.ResendConfirmEmail.Events
{
    public sealed record ConfirmEmailTokenCreatedEvent(ConfirmEmailTokenResult TokenResult) : INotification;
}
