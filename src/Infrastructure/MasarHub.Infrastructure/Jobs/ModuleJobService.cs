using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Infrastructure.Jobs
{
    public sealed class ModuleJobService : IModuleJobService
    {
        private readonly IRepository<CourseAnnouncement> _announcementRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ModuleJobService(IRepository<CourseAnnouncement> announcementRepository, IRepository<Course> courseRepository, IUnitOfWork unitOfWork)
        {
            _announcementRepository = announcementRepository;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAnnouncementForNewModuleAsync(Guid courseId, string moduleTitle)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                return;

            string title = course.Language switch
            {
                CourseLanguage.English => $"New Module Added: {moduleTitle}",
                CourseLanguage.Arabic => $"تم إضافة وحدة جديدة: {moduleTitle}",
                _ => $"New Module Added: {moduleTitle}"
            };
            string content = course.Language switch
            {
                CourseLanguage.English => $"A new module titled '{moduleTitle}' has been added to the course '{course.Title}'.",
                CourseLanguage.Arabic => $"تم إضافة وحدة جديدة بعنوان '{moduleTitle}' إلى الدورة '{course.Title}'.",
                _ => $"A new module titled '{moduleTitle}' has been added to the course '{course.Title}'."
            };

            var announcementResult = CourseAnnouncement.Create(
                courseId,
                course.InstructorId,
                title,
                content,
                AnnouncementImportance.Normal
            );

            if (announcementResult.IsFailure)
                return;

            var announcement = announcementResult.Value;
            announcement.Publish();

            await _announcementRepository.AddAsync(announcement, CancellationToken.None);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
        }
    }
}
