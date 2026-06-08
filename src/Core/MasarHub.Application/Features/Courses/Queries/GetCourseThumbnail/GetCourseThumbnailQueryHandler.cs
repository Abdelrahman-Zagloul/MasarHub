using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Courses.Queries.GetCourseThumbnail
{
    public sealed class GetCourseThumbnailQueryHandler : IRequestHandler<GetCourseThumbnailQuery, Result<CourseThumbnailResponse>>
    {
        private readonly ICourseQuery _courseQuery;
        private readonly IFileStorageService _fileStorageService;

        public GetCourseThumbnailQueryHandler(ICourseQuery courseQuery, IFileStorageService fileStorageService)
        {
            _courseQuery = courseQuery;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<CourseThumbnailResponse>> Handle(GetCourseThumbnailQuery request, CancellationToken cancellationToken)
        {
            var (courseExists, thumbnailPublicId) = await _courseQuery.GetThumbnailDetailsAsync(request.Id, cancellationToken);

            if (!courseExists)
                return Error.NotFound("course.not_found");

            if (string.IsNullOrWhiteSpace(thumbnailPublicId))
                return Error.NotFound("course.thumbnail_not_found");

            var url = _fileStorageService.GetUrl(thumbnailPublicId, FileType.Image);
            return new CourseThumbnailResponse(url);
        }
    }
}
