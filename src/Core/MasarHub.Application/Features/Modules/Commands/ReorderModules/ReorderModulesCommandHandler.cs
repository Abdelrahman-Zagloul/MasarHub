using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Modules.Commands.ReorderModules
{
    public sealed class ReorderModulesCommandHandler : IRequestHandler<ReorderModulesCommand, Result>
    {
        private readonly IRepository<CourseModule> _moduleRepository;
        private readonly ICourseModuleQuery _courseModuleQuery;
        private readonly IUnitOfWork _unitOfWork;

        public ReorderModulesCommandHandler(IRepository<CourseModule> moduleRepository, ICourseModuleQuery courseModuleQuery, IUnitOfWork unitOfWork)
        {
            _moduleRepository = moduleRepository;
            _courseModuleQuery = courseModuleQuery;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ReorderModulesCommand request, CancellationToken cancellationToken)
        {
            var isOwner = await _courseModuleQuery.IsCourseOwnerAsync(request.CourseId, request.InstructorId, cancellationToken);
            if (!isOwner)
                return Error.Forbidden("course.access_denied");

            var modules = await _moduleRepository.GetAllAsync(x => x.CourseId == request.CourseId, cancellationToken);

            if (modules.Count != request.OrderedModuleIds.Count)
                return Error.BadRequest("module.reorder_items_mismatch");

            var moduleMap = modules.ToDictionary(x => x.Id);
            for (int i = 0; i < request.OrderedModuleIds.Count; i++)
            {
                var moduleId = request.OrderedModuleIds[i];
                if (!moduleMap.TryGetValue(moduleId, out var module))
                    return Error.BadRequest("module.reorder_module_not_found");

                var result = module.ChangeDisplayOrder(i + 1);
                if (result.IsFailure)
                    return result.Error;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}