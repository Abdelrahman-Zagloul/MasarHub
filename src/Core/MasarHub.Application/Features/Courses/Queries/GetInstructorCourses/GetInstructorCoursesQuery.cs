using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Courses.Queries.GetCourses;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Queries.GetInstructorCourses
{
    public sealed record GetInstructorCoursesQuery
    (
        string? Title,
        Guid? CategoryId,
        CourseLanguage? Language,
        CourseLevel? Level,
        CourseStatus? Status,
        decimal? MinPrice,
        decimal? MaxPrice,
        int PageNumber = 1,
        int PageSize = 10
    ) : IPaginatedQuery, IRequest<Result<PaginatedResult<CourseResponse>>>;
}
