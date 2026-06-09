using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Courses.Queries.GetCourses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Queries.GetInstructorCourses
{
    public sealed class GetInstructorCoursesQueryHandler
        : IRequestHandler<GetInstructorCoursesQuery, Result<PaginatedResult<CourseResponse>>>
    {
        private readonly ICourseQuery _courseQuery;
        private readonly ICurrentUserService _currentUser;

        public GetInstructorCoursesQueryHandler(ICourseQuery courseQuery, ICurrentUserService currentUser)
        {
            _courseQuery = courseQuery;
            _currentUser = currentUser;
        }

        public async Task<Result<PaginatedResult<CourseResponse>>> Handle(GetInstructorCoursesQuery request, CancellationToken cancellationToken)
        {
            var getCoursesQuery = new GetCoursesQuery
            (
                request.Title,
                request.CategoryId,
                _currentUser.UserId,
                request.Language,
                request.Level,
                request.MinPrice,
                request.MaxPrice,
                request.PageNumber,
                request.PageSize
            );

            var pagedResult = await _courseQuery.GetAllAsync(getCoursesQuery, request.Status, cancellationToken);
            return PaginatedResult<CourseResponse>.Create(pagedResult.Items, pagedResult.TotalCount, request.PageNumber, request.PageSize);
        }
    }
}