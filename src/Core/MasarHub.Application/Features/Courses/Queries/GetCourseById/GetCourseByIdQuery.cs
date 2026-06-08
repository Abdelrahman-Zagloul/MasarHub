using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Courses.Queries.GetCourseById
{
    public sealed record GetCourseByIdQuery(Guid Id) : IRequest<Result<CourseDetailsResponse>>;
}