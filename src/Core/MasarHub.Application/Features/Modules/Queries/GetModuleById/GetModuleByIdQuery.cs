using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Modules.Queries.GetModuleById
{
    public sealed record GetModuleByIdQuery(Guid CourseId, Guid ModuleId) : IRequest<Result<ModuleDetailsResponse>>;
}
