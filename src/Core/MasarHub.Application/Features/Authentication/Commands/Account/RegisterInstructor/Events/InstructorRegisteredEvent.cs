using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.RegisterInstructor.Events
{
    public sealed record InstructorRegisteredEvent
    (
         Guid InstructorProfileId,
         Guid UserId,
         string FullName,
         string Email,
         string EmailVerificationToken
    ) : INotification;
}
