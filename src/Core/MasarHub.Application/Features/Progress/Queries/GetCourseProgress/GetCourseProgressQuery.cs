using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Progress.Queries.GetCourseProgress
{
    public sealed record GetCourseProgressQuery
    (
        Guid UserId,
        Guid CourseId
    ) : IRequest<Result<CourseProgressResponse>>;


}
