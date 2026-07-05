using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Domain.UnitTests.Modules.Courses
{
    [Trait("UnitTests.Domain.Courses", "ModuleProgress")]
    public sealed class ModuleProgressTests
    {
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly Guid CourseId = Guid.NewGuid();
        private static readonly Guid ModuleId = Guid.NewGuid();
        private const int TotalLessons = 5;

        #region Create

        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            var result = ModuleProgress.Create(UserId, CourseId, ModuleId, TotalLessons);

            Assert.True(result.IsSuccess);
            Assert.Equal(UserId, result.Value.UserId);
            Assert.Equal(CourseId, result.Value.CourseId);
            Assert.Equal(ModuleId, result.Value.ModuleId);
            Assert.Equal(TotalLessons, result.Value.TotalLessons);
            Assert.Equal(0, result.Value.CompletedLessons);
            Assert.Null(result.Value.CompletedAt);
            Assert.Empty(result.Value.DomainEvents);
        }

        [Fact]
        public void Create_EmptyUserId_ReturnsError()
        {
            var result = ModuleProgress.Create(Guid.Empty, CourseId, ModuleId, TotalLessons);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("userId").Code, result.Error.Code);
        }

        [Fact]
        public void Create_EmptyCourseId_ReturnsError()
        {
            var result = ModuleProgress.Create(UserId, Guid.Empty, ModuleId, TotalLessons);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("courseId").Code, result.Error.Code);
        }

        [Fact]
        public void Create_EmptyModuleId_ReturnsError()
        {
            var result = ModuleProgress.Create(UserId, CourseId, Guid.Empty, TotalLessons);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("moduleId").Code, result.Error.Code);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_InvalidTotalLessons_ReturnsError(int totalLessons)
        {
            var result = ModuleProgress.Create(UserId, CourseId, ModuleId, totalLessons);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.NegativeOrZero("totalLessons").Code, result.Error.Code);
        }

        #endregion

        #region MarkLessonCompleted

        [Fact]
        public void MarkLessonCompleted_IncrementsCompletedLessons()
        {
            var moduleProgress = CreateValidModuleProgress();

            moduleProgress.MarkLessonCompleted();

            Assert.Equal(1, moduleProgress.CompletedLessons);
            Assert.Null(moduleProgress.CompletedAt);
        }

        [Fact]
        public void MarkLessonCompleted_AllLessonsCompleted_SetsCompletedAt()
        {
            var moduleProgress = CreateValidModuleProgress();

            for (int i = 0; i < TotalLessons; i++)
                moduleProgress.MarkLessonCompleted();

            Assert.Equal(TotalLessons, moduleProgress.CompletedLessons);
            Assert.NotNull(moduleProgress.CompletedAt);
        }

        [Fact]
        public void MarkLessonCompleted_ExceedsTotal_StillIncrements()
        {
            var moduleProgress = CreateValidModuleProgress();

            for (int i = 0; i < TotalLessons + 2; i++)
                moduleProgress.MarkLessonCompleted();

            Assert.Equal(TotalLessons + 2, moduleProgress.CompletedLessons);
        }

        #endregion

        #region UpdateTotalLessons

        [Fact]
        public void UpdateTotalLessons_ValidInput_ReturnsSuccess()
        {
            var moduleProgress = CreateValidModuleProgress();
            var newTotal = 10;

            var result = moduleProgress.UpdateTotalLessons(newTotal);

            Assert.True(result.IsSuccess);
            Assert.Equal(newTotal, moduleProgress.TotalLessons);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void UpdateTotalLessons_InvalidInput_ReturnsError(int totalLessons)
        {
            var moduleProgress = CreateValidModuleProgress();

            var result = moduleProgress.UpdateTotalLessons(totalLessons);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void UpdateTotalLessons_LessThanCompleted_ClampsCompletedLessons()
        {
            var moduleProgress = CreateValidModuleProgress();
            moduleProgress.MarkLessonCompleted();
            moduleProgress.MarkLessonCompleted();
            moduleProgress.MarkLessonCompleted();

            var result = moduleProgress.UpdateTotalLessons(2);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, moduleProgress.TotalLessons);
            Assert.Equal(2, moduleProgress.CompletedLessons);
        }

        #endregion

        #region Reset

        [Fact]
        public void Reset_SetsCompletedLessonsToZero()
        {
            var moduleProgress = CreateValidModuleProgress();
            for (int i = 0; i < TotalLessons; i++)
                moduleProgress.MarkLessonCompleted();

            moduleProgress.Reset();

            Assert.Equal(0, moduleProgress.CompletedLessons);
            Assert.Null(moduleProgress.CompletedAt);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_ReturnsSuccess()
        {
            var moduleProgress = CreateValidModuleProgress();

            var result = moduleProgress.Delete();

            Assert.True(result.IsSuccess);
            Assert.True(moduleProgress.IsDeleted);
            Assert.NotNull(moduleProgress.DeletedAt);
        }

        #endregion

        private static ModuleProgress CreateValidModuleProgress()
        {
            return ModuleProgress.Create(UserId, CourseId, ModuleId, TotalLessons).Value;
        }
    }
}
