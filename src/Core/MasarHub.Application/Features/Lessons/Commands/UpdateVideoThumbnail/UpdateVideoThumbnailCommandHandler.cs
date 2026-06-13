using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses.Lessons;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.UpdateVideoThumbnail
{
    public sealed class UpdateVideoThumbnailCommandHandler : IRequestHandler<UpdateVideoThumbnailCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILessonQuery _lessonQuery;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IFileStorageService _fileStorageService;

        public UpdateVideoThumbnailCommandHandler(IUnitOfWork unitOfWork, ILessonQuery lessonQuery, IRepository<Lesson> lessonRepository, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _lessonQuery = lessonQuery;
            _lessonRepository = lessonRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result> Handle(UpdateVideoThumbnailCommand request, CancellationToken cancellationToken)
        {
            var courseState = await _lessonQuery.GetCourseStateAsync(request.ModuleId, request.InstructorId, cancellationToken);

            if (!courseState.ModuleExist)
                return Error.NotFound("module.not_found");

            if (!courseState.IsOwner)
                return Error.Forbidden("course.access_denied");

            var lesson = await _lessonRepository.GetByIdAsync(request.LessonId);
            if (lesson is null)
                return Error.NotFound("lesson.not_found");

            if (lesson is not VideoLesson videoLesson)
                return Error.BadRequest("lesson.not_video_lesson");

            var uploadResult = await _fileStorageService.UploadAsync(request.File, FileType.Image, StorageFolders.Courses.Thumbnails, cancellationToken);
            if (uploadResult.IsFailure)
                return uploadResult.Errors[0];

            videoLesson.UpdateThumbnail(uploadResult.Value.FileKey);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
