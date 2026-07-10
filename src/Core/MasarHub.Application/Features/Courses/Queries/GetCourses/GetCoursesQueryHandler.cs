using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Queries.GetCourses
{
    public sealed class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, Result<PaginatedResult<CourseResponse>>>
    {
        private readonly ICourseQuery _courseQuery;
        private readonly IFileStorageService _fileStorageService;

        public GetCoursesQueryHandler(ICourseQuery courseQuery, IFileStorageService fileStorageService)
        {
            _courseQuery = courseQuery;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<PaginatedResult<CourseResponse>>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _courseQuery.GetAllAsync(request, CourseStatus.Published, cancellationToken);

            foreach (var course in pagedResult.Items)
            {
                if (!string.IsNullOrWhiteSpace(course.ThumbnailPublicId))
                    course.ThumbnailUrl = _fileStorageService.GetUrl(course.ThumbnailPublicId, FileType.Image);
            }

            return PaginatedResult<CourseResponse>.Create(pagedResult.Items, pagedResult.TotalCount, request.PageNumber, request.PageSize);
        }
    }
}