using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Courses.Lessons;

namespace MasarHub.Domain.UnitTests.Modules.Courses.Lessons
{
    public sealed class ArticleLessonTests
    {
        private static readonly Guid ModuleId = Guid.NewGuid();
        private const string Title = "Introduction to C#";
        private const int DisplayOrder = 1;
        private const string Content = "This is the article content that explains the basics.";

        #region Create

        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            var result = ArticleLesson.Create(ModuleId, true, Title, DisplayOrder, null, Content);

            Assert.True(result.IsSuccess);
            Assert.Equal(ModuleId, result.Value.ModuleId);
            Assert.Equal(Title, result.Value.Title);
            Assert.Equal(DisplayOrder, result.Value.DisplayOrder);
            Assert.True(result.Value.IsPreviewable);
            Assert.Equal(Content, result.Value.Content);
            Assert.Equal(LessonStatus.Active, result.Value.LessonStatus);
            Assert.Empty(result.Value.DomainEvents);
        }

        [Fact]
        public void Create_WithDescription_ReturnsSuccess()
        {
            var description = "A comprehensive article";

            var result = ArticleLesson.Create(ModuleId, false, Title, DisplayOrder, description, Content);

            Assert.True(result.IsSuccess);
            Assert.Equal(description, result.Value.Description);
            Assert.False(result.Value.IsPreviewable);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidTitle_ReturnsError(string? title)
        {
            var result = ArticleLesson.Create(ModuleId, true, title!, DisplayOrder, null, Content);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Create_EmptyModuleId_ReturnsError()
        {
            var result = ArticleLesson.Create(Guid.Empty, true, Title, DisplayOrder, null, Content);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("moduleId").Code, result.Error.Code);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_InvalidDisplayOrder_ReturnsError(int order)
        {
            var result = ArticleLesson.Create(ModuleId, true, Title, order, null, Content);

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidContent_ReturnsError(string? content)
        {
            var result = ArticleLesson.Create(ModuleId, true, Title, DisplayOrder, null, content!);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region Update

        [Fact]
        public void UpdateContent_ValidInput_ReturnsSuccess()
        {
            var lesson = CreateValidLesson();
            var newContent = "Updated content here";

            var result = lesson.UpdateContent(newContent);

            Assert.True(result.IsSuccess);
            Assert.Equal(newContent, lesson.Content);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateContent_InvalidInput_ReturnsError(string? content)
        {
            var lesson = CreateValidLesson();

            var result = lesson.UpdateContent(content!);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void UpdateDescription_ValidInput_ReturnsSuccess()
        {
            var lesson = CreateValidLesson();
            var newDescription = "Updated description";

            var result = lesson.UpdateDescription(newDescription);

            Assert.True(result.IsSuccess);
            Assert.Equal(newDescription, lesson.Description);
        }

        [Fact]
        public void UpdateDescription_Null_ClearsDescription()
        {
            var lesson = CreateValidLessonWithDescription();

            var result = lesson.UpdateDescription(null);

            Assert.True(result.IsSuccess);
            Assert.Null(lesson.Description);
        }

        #endregion

        #region Preview

        [Fact]
        public void EnablePreview_NotPreviewable_ReturnsSuccess()
        {
            var lesson = ArticleLesson.Create(ModuleId, false, Title, DisplayOrder, null, Content).Value;

            var result = lesson.EnablePreview();

            Assert.True(result.IsSuccess);
            Assert.True(lesson.IsPreviewable);
        }

        [Fact]
        public void EnablePreview_AlreadyPreviewable_ReturnsError()
        {
            var lesson = CreateValidLesson();

            var result = lesson.EnablePreview();

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void DisablePreview_Previewable_ReturnsSuccess()
        {
            var lesson = CreateValidLesson();

            var result = lesson.DisablePreview();

            Assert.True(result.IsSuccess);
            Assert.False(lesson.IsPreviewable);
        }

        [Fact]
        public void DisablePreview_AlreadyDisabled_ReturnsError()
        {
            var lesson = ArticleLesson.Create(ModuleId, false, Title, DisplayOrder, null, Content).Value;

            var result = lesson.DisablePreview();

            Assert.True(result.IsFailure);
        }

        #endregion

        #region Archive/Unarchive

        [Fact]
        public void Archive_PublishedCourse_ReturnsSuccess()
        {
            var lesson = CreateValidLesson();

            var result = lesson.Archive(CourseStatus.Published);

            Assert.True(result.IsSuccess);
            Assert.Equal(LessonStatus.Archived, lesson.LessonStatus);
        }

        [Fact]
        public void Archive_UnpublishedCourse_ReturnsError()
        {
            var lesson = CreateValidLesson();

            var result = lesson.Archive(CourseStatus.Draft);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Archive_AlreadyArchived_ReturnsError()
        {
            var lesson = CreateValidLesson();
            lesson.Archive(CourseStatus.Published);

            var result = lesson.Archive(CourseStatus.Published);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Unarchive_ArchivedLesson_ReturnsSuccess()
        {
            var lesson = CreateValidLesson();
            lesson.Archive(CourseStatus.Published);

            var result = lesson.Unarchive(CourseStatus.Published);

            Assert.True(result.IsSuccess);
            Assert.Equal(LessonStatus.Active, lesson.LessonStatus);
        }

        [Fact]
        public void Unarchive_NotArchived_ReturnsError()
        {
            var lesson = CreateValidLesson();

            var result = lesson.Unarchive(CourseStatus.Published);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_DraftCourse_ReturnsSuccess()
        {
            var lesson = CreateValidLesson();

            var result = lesson.Delete(CourseStatus.Draft);

            Assert.True(result.IsSuccess);
            Assert.True(lesson.IsDeleted);
            Assert.NotNull(lesson.DeletedAt);
            Assert.Equal(0, lesson.DisplayOrder);
            Assert.Single(lesson.DomainEvents);
        }

        [Fact]
        public void Delete_PublishedCourse_ReturnsError()
        {
            var lesson = CreateValidLesson();

            var result = lesson.Delete(CourseStatus.Published);

            Assert.True(result.IsFailure);
            Assert.False(lesson.IsDeleted);
        }

        #endregion

        private static ArticleLesson CreateValidLesson()
        {
            return ArticleLesson.Create(ModuleId, true, Title, DisplayOrder, null, Content).Value;
        }

        private static ArticleLesson CreateValidLessonWithDescription()
        {
            return ArticleLesson.Create(ModuleId, true, Title, DisplayOrder, "Original description", Content).Value;
        }
    }
}
