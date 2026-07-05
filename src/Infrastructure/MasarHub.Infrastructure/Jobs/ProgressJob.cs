using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Infrastructure.Jobs
{
    public sealed class ProgressJob : IProgressJob
    {
        private readonly IProgressQuery _progressQuery;
        private readonly IRepository<ModuleProgress> _moduleProgressRepository;
        private readonly IRepository<CourseProgress> _courseProgressRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProgressJob(
            IProgressQuery progressQuery,
            IRepository<ModuleProgress> moduleProgressRepository,
            IRepository<CourseProgress> courseProgressRepository,
            IUnitOfWork unitOfWork)
        {
            _progressQuery = progressQuery;
            _moduleProgressRepository = moduleProgressRepository;
            _courseProgressRepository = courseProgressRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task UpdateProgressAsync(Guid userId, Guid courseId, Guid moduleId, Guid lessonId)
        {
            await UpdateModuleProgressAsync(userId, courseId, moduleId);
            await UpdateCourseProgressAsync(userId, courseId);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task UpdateModuleProgressAsync(Guid userId, Guid courseId, Guid moduleId)
        {
            var moduleProgress = await _moduleProgressRepository.GetAsync(x => x.UserId == userId && x.ModuleId == moduleId && x.CourseId == courseId);
            if (moduleProgress != null)
            {
                moduleProgress.MarkLessonCompleted();
                return;
            }

            var totalLessons = await _progressQuery.GetModuleLessonCountAsync(moduleId);
            var createResult = ModuleProgress.Create(userId, courseId, moduleId, totalLessons);
            if (createResult.IsFailure)
                return;

            moduleProgress = createResult.Value;
            moduleProgress.MarkLessonCompleted();
            await _moduleProgressRepository.AddAsync(moduleProgress);
        }

        private async Task UpdateCourseProgressAsync(Guid userId, Guid courseId)
        {
            var courseProgress = await _courseProgressRepository.GetAsync(x => x.UserId == userId && x.CourseId == courseId);
            if (courseProgress != null)
            {
                courseProgress.MarkLessonCompleted();
                return;
            }

            var allLessons = await _progressQuery.GetCourseLessonCountAsync(courseId);
            var createResult = CourseProgress.Create(userId, courseId, allLessons);
            if (createResult.IsFailure)
                return;

            courseProgress = createResult.Value;
            courseProgress.MarkLessonCompleted();
            await _courseProgressRepository.AddAsync(courseProgress);
        }
    }
}
