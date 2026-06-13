using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCourseThumbnail
{
    public sealed class UpdateCourseThumbnailCommandHandler : IRequestHandler<UpdateCourseThumbnailCommand, Result<string>>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorageService _fileStorageService;

        public UpdateCourseThumbnailCommandHandler(IRepository<Course> courseRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFileStorageService fileStorageService)
        {
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<string>> Handle(UpdateCourseThumbnailCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course is null)
                return Error.NotFound("course.not_found");

            if (course.InstructorId != _currentUserService.UserId)
                return Error.Forbidden("course.access_denied");

            var oldThumbnailPublicId = course.ThumbnailPublicId;
            var storedFileResult = await _fileStorageService.UploadAsync(request.File, FileType.Image, StorageFolders.Courses.Thumbnails, cancellationToken);
            if (storedFileResult.IsFailure)
                return storedFileResult.Errors[0];

            course.UpdateThumbnailPublicId(storedFileResult.Value.FileKey);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(oldThumbnailPublicId))
                await _fileStorageService.DeleteAsync(oldThumbnailPublicId, FileType.Image, cancellationToken);

            return storedFileResult.Value.Url;
        }
    }
}