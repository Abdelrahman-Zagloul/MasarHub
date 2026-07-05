using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Courses.Events;
using MasarHub.Domain.Modules.Courses.Lessons;

namespace MasarHub.Domain.UnitTests.Modules.Courses.Lessons
{
    [Trait("UnitTests.Domain.Lessons", "LessonProgress")]
    public sealed class LessonProgressTests
    {
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly Guid LessonId = Guid.NewGuid();
        private static readonly Guid ModuleId = Guid.NewGuid();
        private static readonly Guid CourseId = Guid.NewGuid();

        #region Create

        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            var result = LessonProgress.Create(UserId, LessonId, ModuleId, CourseId);

            Assert.True(result.IsSuccess);
            Assert.Equal(UserId, result.Value.UserId);
            Assert.Equal(LessonId, result.Value.LessonId);
            Assert.Equal(ModuleId, result.Value.ModuleId);
            Assert.Equal(CourseId, result.Value.CourseId);
            var domainEvent = Assert.Single(result.Value.DomainEvents);
            Assert.IsType<LessonProgressCreatedDomainEvent>(domainEvent);
            var createdEvent = (LessonProgressCreatedDomainEvent)domainEvent;
            Assert.Equal(UserId, createdEvent.UserId);
            Assert.Equal(CourseId, createdEvent.CourseId);
            Assert.Equal(ModuleId, createdEvent.ModuleId);
            Assert.Equal(LessonId, createdEvent.LessonId);
        }

        [Fact]
        public void Create_EmptyUserId_ReturnsError()
        {
            var result = LessonProgress.Create(Guid.Empty, LessonId, ModuleId, CourseId);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("userId").Code, result.Error.Code);
        }

        [Fact]
        public void Create_EmptyLessonId_ReturnsError()
        {
            var result = LessonProgress.Create(UserId, Guid.Empty, ModuleId, CourseId);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("lessonId").Code, result.Error.Code);
        }

        [Fact]
        public void Create_EmptyModuleId_ReturnsError()
        {
            var result = LessonProgress.Create(UserId, LessonId, Guid.Empty, CourseId);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("moduleId").Code, result.Error.Code);
        }

        [Fact]
        public void Create_EmptyCourseId_ReturnsError()
        {
            var result = LessonProgress.Create(UserId, LessonId, ModuleId, Guid.Empty);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("courseId").Code, result.Error.Code);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_ReturnsSuccess()
        {
            var lessonProgress = CreateValidLessonProgress();

            var result = lessonProgress.Delete();

            Assert.True(result.IsSuccess);
            Assert.True(lessonProgress.IsDeleted);
            Assert.NotNull(lessonProgress.DeletedAt);
        }

        #endregion

        private static LessonProgress CreateValidLessonProgress()
        {
            return LessonProgress.Create(UserId, LessonId, ModuleId, CourseId).Value;
        }
    }
}
