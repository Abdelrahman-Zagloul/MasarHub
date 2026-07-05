using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Progress.Queries.GetCourseProgress
{
    public sealed class GetCourseProgressQueryHandler : IRequestHandler<GetCourseProgressQuery, Result<CourseProgressResponse>>
    {
        private readonly IProgressQuery _progressQuery;

        public GetCourseProgressQueryHandler(IProgressQuery progressQuery)
        {
            _progressQuery = progressQuery;
        }

        public async Task<Result<CourseProgressResponse>> Handle(GetCourseProgressQuery request, CancellationToken cancellationToken)
        {
            var data = await _progressQuery.GetCourseProgressAsync(request.UserId, request.CourseId, cancellationToken);
            if (!data.HasEnrollment)
                return Error.Forbidden("course.not_enrolled");

            if (data.CourseProgress == null)
            {
                var lessonsCount = await _progressQuery.GetCourseLessonCountAsync(request.CourseId, cancellationToken);
                return new CourseProgressResponse(request.CourseId, 0, lessonsCount, false, null, []);
            }

            return new CourseProgressResponse
            (
                request.CourseId,
                data.CourseProgress.CompletedLessons,
                data.CourseProgress.TotalLessons,
                data.CourseProgress.CompletedAt.HasValue,
                data.CourseProgress.CompletedAt,
                data.Modules
            );
        }
    }
}
