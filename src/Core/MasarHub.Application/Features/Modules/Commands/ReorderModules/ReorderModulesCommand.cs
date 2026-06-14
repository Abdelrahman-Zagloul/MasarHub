using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Modules.Commands.ReorderModules
{
    public sealed record ReorderModulesCommand
    (
         Guid CourseId,
         Guid InstructorId,
         IReadOnlyList<Guid> OrderedModuleIds
    ) : IRequest<Result>;
}
