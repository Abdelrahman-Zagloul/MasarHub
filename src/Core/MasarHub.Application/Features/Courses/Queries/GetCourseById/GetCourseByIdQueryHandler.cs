using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Courses.Queries.GetCourseById
{
    public sealed class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, Result<CourseDetailsResponse>>
    {
        private readonly ICourseQuery _courseQuery;

        public GetCourseByIdQueryHandler(ICourseQuery courseQuery)
        {
            _courseQuery = courseQuery;
        }

        public async Task<Result<CourseDetailsResponse>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _courseQuery.GetDetailsByIdAsync(request.Id, cancellationToken);
            if (course == null)
                return Error.NotFound("course.not_found");

            return course;
        }
    }
}