using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.ResendConfirmEmail.Events
{
    public sealed record ConfirmEmailTokenCreatedEvent(ConfirmEmailTokenResult TokenResult) : INotification;
}
