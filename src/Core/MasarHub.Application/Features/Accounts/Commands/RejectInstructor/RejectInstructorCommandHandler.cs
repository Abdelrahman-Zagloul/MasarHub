using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Profiles;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Commands.RejectInstructor
{
    public sealed class RejectInstructorCommandHandler : IRequestHandler<RejectInstructorCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<InstructorProfile> _repository;

        public RejectInstructorCommandHandler(IUnitOfWork unitOfWork, IRepository<InstructorProfile> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<Result> Handle(RejectInstructorCommand request, CancellationToken cancellationToken)
        {
            var profile = await _repository.GetAsync(x => x.UserId == request.InstructorUserId, cancellationToken);
            if (profile == null)
                return Error.NotFound(ProfileErrors.ProfileNotFound.Code);

            var result = profile.Reject(request.AdminUserId, request.Reason);
            if (result.IsFailure)
                return result.Error;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
