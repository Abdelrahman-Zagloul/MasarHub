using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.RegisterStudent
{
    public sealed record StudentRegisteredEvent(string FullName, string Email, string Token) : INotification;
}
