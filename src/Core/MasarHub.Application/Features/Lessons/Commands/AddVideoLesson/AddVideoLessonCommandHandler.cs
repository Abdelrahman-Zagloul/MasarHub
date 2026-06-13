using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
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
            var creationData = await _lessonQuery.GetCreationDataAsync(request.ModuleId, request.InstructorId, cancellationToken);

            if (!creationData.ModuleExist)
                return Error.NotFound("module.not_found");

            if (!creationData.IsOwner)
                return Error.Forbidden("course.access_denied");


            var metadataResult = await _fileStorageService.GetVideoMetadataAsync(request.FileKey, cancellationToken);
            if (metadataResult.IsFailure)
                return metadataResult.Errors[0];


            var lessonResult = VideoLesson.Create(
                request.ModuleId,
                request.IsPreviewable,
                request.Title,
                creationData.NextDisplayOrder,
                request.Description,
                request.FileKey,
                metadataResult.Value.FileName,
                metadataResult.Value.FileSizeInByte,
                metadataResult.Value.DurationInSecond
            );

            if (lessonResult.IsFailure)
                return lessonResult.Error;

            await _lessonRepository.AddAsync(lessonResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AddVideoLessonResponse(
                lessonResult.Value.Id,
                request.ModuleId,
                request.IsPreviewable,
                creationData.NextDisplayOrder,
                request.Title,
                request.Description,
                metadataResult.Value.Url,
                metadataResult.Value.FileName,
                metadataResult.Value.FileSizeInByte,
                metadataResult.Value.DurationInSecond
            );
        }
    }
}
