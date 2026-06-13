using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Queries.GetVideoUploadSignature
{
    public sealed class GetVideoUploadSignatureQueryHandler : IRequestHandler<GetVideoUploadSignatureQuery, Result<UploadSignatureParams>>
    {
        private readonly ILessonQuery _lessonQuery;
        private readonly IFileStorageService _fileStorageService;

        public GetVideoUploadSignatureQueryHandler(ILessonQuery lessonQuery, IFileStorageService fileStorageService)
        {
            _lessonQuery = lessonQuery;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<UploadSignatureParams>> Handle(GetVideoUploadSignatureQuery request, CancellationToken cancellationToken)
        {

            var moduleAccessData = await _lessonQuery.GetModuleAccessDataAsync(request.ModuleId, request.InstructorId, cancellationToken);

            if (!moduleAccessData.ModuleExist)
                return Error.NotFound("module.not_found");

            if (!moduleAccessData.IsOwner)
                return Error.Forbidden("course.access_denied");

            var signature = _fileStorageService.GenerateUploadSignature(FileType.Video, StorageFolders.Courses.Videos);
            return signature;
        }
    }
}
