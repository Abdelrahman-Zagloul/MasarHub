using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Modules.Commands.DeleteModule
{
    public sealed record DeleteModuleCommand(Guid CourseId, Guid ModuleId, Guid InstructorId) : IRequest<Result>;
}
