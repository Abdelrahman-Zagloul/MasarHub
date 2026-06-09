using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Modules.Commands.CreateModule
{
    public sealed class CreateModuleCommandHandler
         : IRequestHandler<CreateModuleCommand, Result<CreateModuleResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseModuleQuery _moduleQuery;
        private readonly IRepository<CourseModule> _moduleRepository;

        public CreateModuleCommandHandler(IUnitOfWork unitOfWork, ICourseModuleQuery moduleQuery, IRepository<CourseModule> moduleRepository)
        {
            _unitOfWork = unitOfWork;
            _moduleQuery = moduleQuery;
            _moduleRepository = moduleRepository;
        }

        public async Task<Result<CreateModuleResponse>> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
        {
            var (courseExists, isOwner, nextDisplayOrder) = await _moduleQuery.GetCreationDataAsync(request.CourseId, request.InstructorId, cancellationToken);
            if (!courseExists)
                return Error.NotFound("course.not_found");

            if (!isOwner)
                return Error.Forbidden("course.access_denied");

            var moduleResult = CourseModule.Create(request.CourseId, request.Title, nextDisplayOrder, request.Description);
            if (moduleResult.IsFailure)
                return moduleResult.Error;

            var courseModule = moduleResult.Value;

            await _moduleRepository.AddAsync(courseModule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateModuleResponse(
                courseModule.Id,
                courseModule.CourseId,
                courseModule.Title,
                courseModule.DisplayOrder,
                courseModule.Description);
        }
    }
}