using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Profiles;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Commands.ApproveInstructor
{
    public sealed class ApproveInstructorCommandHandler : IRequestHandler<ApproveInstructorCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<InstructorProfile> _repository;

        public ApproveInstructorCommandHandler(IUnitOfWork unitOfWork, IRepository<InstructorProfile> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<Result> Handle(ApproveInstructorCommand request, CancellationToken cancellationToken)
        {
            var profile = await _repository.GetAsync(x => x.UserId == request.InstructorUserId, cancellationToken);
            if (profile == null)
                return Error.NotFound(ProfileErrors.ProfileNotFound.Code);

            var result = profile.Approve(request.AdminUserId);
            if (result.IsFailure)
                return result.Error;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
