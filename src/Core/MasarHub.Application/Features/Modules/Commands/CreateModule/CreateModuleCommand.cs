using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Modules.Commands.CreateModule
{
    public sealed record CreateModuleCommand
    (
        Guid CourseId,
        Guid InstructorId,
        string Title,
        string? Description = null
    ) : IRequest<Result<CreateModuleResponse>>;

}
