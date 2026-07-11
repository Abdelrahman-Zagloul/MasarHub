using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Courses.Queries.GetCourseById
{
    public sealed class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, Result<CourseDetailsResponse>>
    {
        private readonly ICourseQuery _courseQuery;
        private readonly IFileStorageService _fileStorageService;

        public GetCourseByIdQueryHandler(ICourseQuery courseQuery, IFileStorageService fileStorageService)
        {
            _courseQuery = courseQuery;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<CourseDetailsResponse>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _courseQuery.GetDetailsByIdAsync(request.Id, cancellationToken);
            if (course == null)
                return Error.NotFound("course.not_found");

            if (!string.IsNullOrWhiteSpace(course.ThumbnailPublicId))
                course.ThumbnailUrl = _fileStorageService.GetUrl(course.ThumbnailPublicId, FileType.Image);

            course.ModuleCount = course.Modules.Count;
            course.LessonCount = course.Modules.Sum(m => m.LessonCount);

            return course;
        }
    }
}