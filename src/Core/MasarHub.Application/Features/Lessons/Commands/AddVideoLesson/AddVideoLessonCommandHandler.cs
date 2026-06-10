using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses.Lessons;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.AddVideoLesson
{
    public sealed class AddVideoLessonCommandHandler : IRequestHandler<AddVideoLessonCommand, Result<AddVideoLessonResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly ILessonQuery _lessonQuery;
        private readonly IFileStorageService _fileStorageService;

        public AddVideoLessonCommandHandler(IUnitOfWork unitOfWork, IRepository<Lesson> lessonRepository, ILessonQuery lessonQuery, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _lessonRepository = lessonRepository;
            _lessonQuery = lessonQuery;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<AddVideoLessonResponse>> Handle(AddVideoLessonCommand request, CancellationToken cancellationToken)
        {
            var creationData = await _lessonQuery
                .GetCreationDataAsync(request.CourseId, request.ModuleId, request.InstructorId, cancellationToken);

            if (!creationData.IsOwner)
                return Error.Forbidden("course.access_denied");

            if (!creationData.ModuleExist)
                return Error.NotFound("module.not_found");

            var uploadResult = await _fileStorageService.UploadAsync(request.VideoFile, FileType.Video, StorageFolders.Courses.Videos);
            if (uploadResult.IsFailure)
                return uploadResult.Errors[0];

            var lessonResult = VideoLesson.Create(
                request.ModuleId,
                request.IsPreviewable,
                request.Title,
                creationData.NextDisplayOrder,
                request.Description,
                uploadResult.Value.FileKey,
                request.VideoFile.FileName,
                request.VideoFile.FileSizeInByte,
                uploadResult.Value.DurationInSecond
            );

            if (lessonResult.IsFailure)
            {
                await _fileStorageService.DeleteAsync(uploadResult.Value.FileKey, FileType.Video, cancellationToken);
                return lessonResult.Error;
            }

            await _lessonRepository.AddAsync(lessonResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AddVideoLessonResponse(
                lessonResult.Value.Id,
                request.ModuleId,
                request.IsPreviewable,
                creationData.NextDisplayOrder,
                request.Title,
                request.Description,
                uploadResult.Value.Url,
                request.VideoFile.FileName,
                request.VideoFile.FileSizeInByte,
                uploadResult.Value.DurationInSecond
            );
        }
    }
}
