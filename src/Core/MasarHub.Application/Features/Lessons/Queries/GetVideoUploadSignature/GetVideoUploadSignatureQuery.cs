using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Queries.GetVideoUploadSignature
{
    public sealed record GetVideoUploadSignatureQuery(Guid ModuleId, Guid InstructorId) : IRequest<Result<UploadSignatureParams>>;
}
