using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Courses.Queries.GetCourseThumbnail
{
    public sealed record GetCourseThumbnailQuery(Guid Id) : IRequest<Result<CourseThumbnailResponse>>;
}
