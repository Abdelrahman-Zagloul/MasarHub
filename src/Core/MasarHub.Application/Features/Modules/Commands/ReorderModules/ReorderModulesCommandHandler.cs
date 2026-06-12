using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Modules.Commands.ReorderModules
{
    public sealed class ReorderModulesCommandHandler : IRequestHandler<ReorderModulesCommand, Result>
    {
        private readonly ICourseModuleQuery _courseModuleQuery;
        public ReorderModulesCommandHandler(ICourseModuleQuery courseModuleQuery)
        {
            _courseModuleQuery = courseModuleQuery;
        }

        public async Task<Result> Handle(ReorderModulesCommand request, CancellationToken cancellationToken)
        {
            var isOwner = await _courseModuleQuery.IsCourseOwnerAsync(request.CourseId, request.InstructorId, cancellationToken);
            if (!isOwner)
                return Error.Forbidden("course.access_denied");

            var existingModuleIds = await _courseModuleQuery.GetModuleIdsByCourseIdAsync(request.CourseId, cancellationToken);

            if (existingModuleIds.Count != request.OrderedModuleIds.Count)
                return Error.BadRequest("module.reorder_items_mismatch");

            var orderedModuleIdsSet = request.OrderedModuleIds.ToHashSet();
            if (!existingModuleIds.All(id => orderedModuleIdsSet.Contains(id)))
                return Error.BadRequest("module.reorder_module_not_found");

            bool isSuccess = await _courseModuleQuery.BulkUpdateDisplayOrderAsync(request.CourseId, request.OrderedModuleIds, cancellationToken);

            if (!isSuccess)
                return Error.Failure("module.reorder_failed");

            return Result.Success();
        }
    }
}