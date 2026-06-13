using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Courses.Lessons;
using MediatR;

namespace MasarHub.Application.Features.Attachments.Commands.AddAttachment
{
    public sealed class AddAttachmentCommandHandler
        : IRequestHandler<AddAttachmentCommand, Result<AddAttachmentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILessonQuery _lessonQuery;
        private readonly IRepository<LessonAttachment> _lessonAttachmentRepository;

        public AddAttachmentCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService, ILessonQuery lessonQuery, IRepository<LessonAttachment> lessonAttachmentRepository)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _lessonQuery = lessonQuery;
            _lessonAttachmentRepository = lessonAttachmentRepository;
        }

        public async Task<Result<AddAttachmentResponse>> Handle(AddAttachmentCommand request, CancellationToken cancellationToken)
        {
            var creationData = await _lessonQuery.GetLessonAttachmentCreationAsync(
                request.LessonId, request.InstructorId, cancellationToken);

            if (!creationData.LessonExist)
                return Error.NotFound("lesson.not_found");

            if (!creationData.IsOwner)
                return Error.Forbidden("course.access_denied");

            var canAddMoreResult = LessonAttachment.CanAddMoreAttachment(creationData.AttachmentCount);
            if (canAddMoreResult.IsFailure)
                return Error.Conflict(canAddMoreResult.Error.Code);


            var uploadResult = await _fileStorageService.UploadAsync(request.File, FileType.Attachment, StorageFolders.Courses.Attachments, cancellationToken);
            if (uploadResult.IsFailure)
                return uploadResult.Errors[0];

            var attachmentResult = LessonAttachment.Create(
                request.LessonId,
                uploadResult.Value.FileKey,
                uploadResult.Value.FileName,
                uploadResult.Value.ContentType,
                uploadResult.Value.FileSizeInByte);

            if (attachmentResult.IsFailure)
            {
                await _fileStorageService.DeleteAsync(uploadResult.Value.FileKey, FileType.Attachment, cancellationToken);
                return attachmentResult.Error;
            }

            await _lessonAttachmentRepository.AddAsync(attachmentResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AddAttachmentResponse(
                 attachmentResult.Value.Id,
                 request.LessonId,
                 uploadResult.Value.Url,
                 attachmentResult.Value.FileName,
                 attachmentResult.Value.FileType,
                 attachmentResult.Value.FileSizeInBytes);
        }
    }
}
