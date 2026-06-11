using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Modules.Commands.UpdateModule
{
    public sealed class UpdateModuleCommandHandler : IRequestHandler<UpdateModuleCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CourseModule> _courseModuleRepository;
        private readonly ICourseModuleQuery _courseModuleQuery;
        public UpdateModuleCommandHandler(IUnitOfWork unitOfWork, IRepository<CourseModule> courseModuleRepository, ICourseModuleQuery courseModuleQuery)
        {
            _unitOfWork = unitOfWork;
            _courseModuleRepository = courseModuleRepository;
            _courseModuleQuery = courseModuleQuery;
        }

        public async Task<Result> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
        {
            var updateData = await _courseModuleQuery.GetUpdateDataAsync(request.CourseId, request.ModuleId, request.InstructorId, cancellationToken);

            if (!updateData.ModuleExists)
                return Error.NotFound("module.not_found");

            if (!updateData.IsOwner)
                return Error.Forbidden("course.access_denied");

            var courseModule = await _courseModuleRepository.GetByIdAsync(request.ModuleId, cancellationToken);
            if (courseModule == null)
                return Error.NotFound("module.not_found");

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                var titleResult = courseModule.UpdateTitle(request.Title);
                if (titleResult.IsFailure)
                    return titleResult.Error;
            }

            courseModule.UpdateDescription(request.Description);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
