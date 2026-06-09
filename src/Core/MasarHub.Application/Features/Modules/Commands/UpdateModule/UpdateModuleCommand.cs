using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Modules.Commands.UpdateModule
{
    public record UpdateModuleCommand
    (
         Guid CourseId,
         Guid ModuleId,
         Guid InstructorId,
         string? Title,
         string? Description
    ) : IRequest<Result>;
}
