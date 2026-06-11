using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Modules.Commands.DeleteModule
{
    public sealed class DeleteModuleCommandHandler : IRequestHandler<DeleteModuleCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseModuleQuery _courseModuleQuery;
        private readonly IRepository<CourseModule> _courseModuleRepository;

        public DeleteModuleCommandHandler(IUnitOfWork unitOfWork, ICourseModuleQuery courseModuleQuery, IRepository<CourseModule> courseModuleRepository)
        {
            _unitOfWork = unitOfWork;
            _courseModuleQuery = courseModuleQuery;
            _courseModuleRepository = courseModuleRepository;
        }

        public async Task<Result> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
        {
            var deleteData = await _courseModuleQuery.GetDeleteDataAsync(request.CourseId, request.ModuleId, request.InstructorId, cancellationToken);

            if (!deleteData.ModuleExists)
                return Error.NotFound("module.not_found");

            if (!deleteData.IsOwner)
                return Error.Forbidden("course.access_denied");

            if (deleteData.HasLessons)
                return Error.BadRequest("module.cannot_delete.has_lessons");

            var courseModule = await _courseModuleRepository.GetByIdAsync(request.ModuleId, cancellationToken);
            if (courseModule == null)
                return Error.NotFound("module.not_found");

            var delateResult = courseModule.Delete();
            if (delateResult.IsFailure)
                return delateResult.Error;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
