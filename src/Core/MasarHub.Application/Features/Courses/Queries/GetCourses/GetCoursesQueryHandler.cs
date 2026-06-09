using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Queries.GetCourses
{
    public sealed class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, Result<PaginatedResult<CourseResponse>>>
    {
        private readonly ICourseQuery _courseQuery;

        public GetCoursesQueryHandler(ICourseQuery courseQuery)
        {
            _courseQuery = courseQuery;
        }

        public async Task<Result<PaginatedResult<CourseResponse>>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _courseQuery.GetAllAsync(request, CourseStatus.Published, cancellationToken);
            return PaginatedResult<CourseResponse>.Create(pagedResult.Items, pagedResult.TotalCount, request.PageNumber, request.PageSize);
        }
    }
}