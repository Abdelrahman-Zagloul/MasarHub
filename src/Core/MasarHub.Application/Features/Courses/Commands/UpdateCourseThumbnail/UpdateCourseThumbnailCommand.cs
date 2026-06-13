using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCourseThumbnail
{
    public sealed record UpdateCourseThumbnailCommand(Guid CourseId, FileResource File) : IRequest<Result<string>>;
}
