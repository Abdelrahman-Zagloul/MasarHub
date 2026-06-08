using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Queries.GetCourses
{
    public sealed record GetCoursesQuery
    (
        string? Title,
        Guid? CategoryId,
        Guid? InstructorId,
        CourseLanguage? Language,
        CourseLevel? Level,
        decimal? MinPrice,
        decimal? MaxPrice,
        int PageNumber = 1,
        int PageSize = 10
    ) : IPaginatedQuery, IRequest<Result<PaginatedResult<CourseResponse>>>;
}