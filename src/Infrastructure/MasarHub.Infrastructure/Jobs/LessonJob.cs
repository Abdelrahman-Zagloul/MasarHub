using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Domain.Modules.Courses.Lessons;

namespace MasarHub.Infrastructure.Jobs
{
    public sealed class LessonJob : ILessonJob
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<LessonAttachment> _attachmentRepository;

        public LessonJob(IFileStorageService fileStorageService, IUnitOfWork unitOfWork, IRepository<Lesson> lessonRepository, IRepository<LessonAttachment> attachmentRepository)
        {
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
            _lessonRepository = lessonRepository;
            _attachmentRepository = attachmentRepository;
        }

        public async Task CleanUpAttachmentResourseAsync(Guid attachmentId)
        {
            var attachment = await _attachmentRepository.GetWithDeletedAsync(x => x.Id == attachmentId && x.IsDeleted);
            if (attachment == null)
                return;

            await _fileStorageService.DeleteAsync(attachment.PublicId, FileType.Attachment);
        }

        public async Task CleanUpVideoThumbnailAsync(string thumbnailPublicId)
        {
            await _fileStorageService.DeleteAsync(thumbnailPublicId, FileType.Image);
        }

        public async Task CleanUpLessonResourseAsync(Guid ModuleId, Guid LessonId)
        {
            var lesson = await _lessonRepository.GetWithDeletedAsync(x => x.Id == LessonId && x.ModuleId == ModuleId && x.IsDeleted);
            if (lesson == null)
                return;

            if (lesson is VideoLesson videoLesson)
            {
                await _fileStorageService.DeleteAsync(videoLesson.VideoPublicId, FileType.Video);
                if (videoLesson.ThumbnailPublicId != null)
                    await _fileStorageService.DeleteAsync(videoLesson.ThumbnailPublicId, FileType.Image);
            }

            var attachments = await _attachmentRepository.GetAllAsync(x => x.LessonId == LessonId);
            if (!attachments.Any())
                return;

            foreach (var attachment in attachments)
            {
                await _fileStorageService.DeleteAsync(attachment.PublicId, FileType.Attachment);
                attachment.Delete();
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
