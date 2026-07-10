using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Modules.Queries.GetModuleById
{
    public sealed class GetModuleByIdQueryHandler : IRequestHandler<GetModuleByIdQuery, Result<ModuleDetailsResponse>>
    {
        private readonly ICourseModuleQuery _courseModuleQuery;
        private readonly IFileStorageService _fileStorageService;

        public GetModuleByIdQueryHandler(ICourseModuleQuery courseModuleQuery, IFileStorageService fileStorageService)
        {
            _courseModuleQuery = courseModuleQuery;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<ModuleDetailsResponse>> Handle(GetModuleByIdQuery request, CancellationToken cancellationToken)
        {
            var module = await _courseModuleQuery.GetModuleByIdAsync(request.CourseId, request.ModuleId, cancellationToken);
            if (module == null)
                return Error.NotFound("module.not_found");

            foreach (var lesson in module.Lessons)
            {
                if (lesson.IsPreviewable && !string.IsNullOrWhiteSpace(lesson.VideoPublicId))
                    lesson.VideoUrl = _fileStorageService.GetUrl(lesson.VideoPublicId, FileType.Video);
            }

            return module;
        }
    }
}
