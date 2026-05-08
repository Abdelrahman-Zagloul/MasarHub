using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Persistence;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.RegisterInstructor.Events;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.RegisterInstructor
{
    public sealed class RegisterInstructorCommandHandler : IRequestHandler<RegisterInstructorCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<InstructorProfile> _repository;

        private readonly IMediator _mediator;

        public RegisterInstructorCommandHandler(IAuthService authService, IMediator mediator, IUnitOfWork unitOfWork, IRepository<InstructorProfile> repository)
        {
            _authService = authService;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<Result> Handle(RegisterInstructorCommand request, CancellationToken cancellationToken)
        {
            var registerUserResult = await _authService.RegisterUserAsync(
                request.FullName,
                request.Email,
                request.Password,
                request.PhoneNumber,
                request.Gender,
                UserRole.Instructor,
                cancellationToken);

            if (registerUserResult.IsFailure)
                return registerUserResult;

            var profileResult = InstructorProfile.Create(registerUserResult.Value.UserId, request.Headline, request.Bio, request.Company);
            if (profileResult.IsFailure)
            {
                await _authService.DeleteUserAsync(registerUserResult.Value.UserId);
                return Error.BadRequest(profileResult.Error.Code);
            }

            var profile = profileResult.Value;
            foreach (var platform in request.SocialLinks)
            {
                var socialLinkResult = SocialLink.Create(platform.PlatformName, platform.Url);
                if (socialLinkResult.IsFailure)
                {
                    await _authService.DeleteUserAsync(registerUserResult.Value.UserId);
                    return Error.BadRequest(socialLinkResult.Error.Code);
                }

                var createProfileResult = profile.AddSocialLink(socialLinkResult.Value);
                if (createProfileResult.IsFailure)
                {
                    await _authService.DeleteUserAsync(registerUserResult.Value.UserId);
                    return Error.BadRequest(createProfileResult.Error.Code);
                }
            }

            await _repository.AddAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            await _mediator.Publish(new InstructorRegisteredEvent(
                    profile.Id,
                    registerUserResult.Value.UserId,
                    request.FullName,
                    request.Email,
                    registerUserResult.Value.EmailVerificationToken), cancellationToken);

            return Result.Success("auth.registration.success");
        }
    }
}
