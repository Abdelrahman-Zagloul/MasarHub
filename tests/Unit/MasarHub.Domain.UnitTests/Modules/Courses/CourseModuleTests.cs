using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Courses.Events;

namespace MasarHub.Domain.UnitTests.Modules.Courses
{
    public sealed class CourseModuleTests
    {
        private static readonly Guid ValidCourseId = Guid.NewGuid();
        private const string ValidTitle = "Introduction";
        private const int ValidDisplayOrder = 1;

        #region Create

        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            var result = CourseModule.Create(ValidCourseId, ValidTitle, ValidDisplayOrder);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(ValidCourseId, result.Value.CourseId);
            Assert.Equal(ValidTitle, result.Value.Title);
            Assert.Equal(ValidDisplayOrder, result.Value.DisplayOrder);
            Assert.Single(result.Value.DomainEvents);
            Assert.IsType<ModuleCreatedDomainEvent>(result.Value.DomainEvents.First());
        }

        [Fact]
        public void Create_EmptyCourseId_ReturnsError()
        {
            var result = CourseModule.Create(Guid.Empty, ValidTitle, ValidDisplayOrder);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("courseId").Code, result.Error.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidTitle_ReturnsError(string? title)
        {
            var result = CourseModule.Create(ValidCourseId, title!, ValidDisplayOrder);

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_InvalidDisplayOrder_ReturnsError(int displayOrder)
        {
            var result = CourseModule.Create(ValidCourseId, ValidTitle, displayOrder);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Create_WithDescription_ReturnsSuccess()
        {
            var description = "This module covers the basics";

            var result = CourseModule.Create(ValidCourseId, ValidTitle, ValidDisplayOrder, description);

            Assert.True(result.IsSuccess);
            Assert.Equal(description, result.Value.Description);
        }

        #endregion

        #region Update

        [Fact]
        public void UpdateTitle_ValidInput_ReturnsSuccess()
        {
            var module = CreateValidModule();
            var newTitle = "Advanced Topics";

            var result = module.UpdateTitle(newTitle);

            Assert.True(result.IsSuccess);
            Assert.Equal(newTitle, module.Title);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateTitle_InvalidInput_ReturnsError(string? title)
        {
            var module = CreateValidModule();

            var result = module.UpdateTitle(title!);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void UpdateDescription_ValidInput_ReturnsSuccess()
        {
            var module = CreateValidModule();
            var newDescription = "Updated module description";

            var result = module.UpdateDescription(newDescription);

            Assert.True(result.IsSuccess);
            Assert.Equal(newDescription, module.Description);
        }

        [Fact]
        public void UpdateDescription_Null_ClearsDescription()
        {
            var module = CreateValidModuleWithDescription();

            var result = module.UpdateDescription(null);

            Assert.True(result.IsSuccess);
            Assert.Null(module.Description);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_ReturnsSuccess()
        {
            var module = CreateValidModule();

            var result = module.Delete();

            Assert.True(result.IsSuccess);
            Assert.True(module.IsDeleted);
            Assert.NotNull(module.DeletedAt);
            Assert.Equal(0, module.DisplayOrder);
        }

        #endregion

        private static CourseModule CreateValidModule()
        {
            return CourseModule.Create(ValidCourseId, ValidTitle, ValidDisplayOrder).Value;
        }

        private static CourseModule CreateValidModuleWithDescription()
        {
            return CourseModule.Create(ValidCourseId, ValidTitle, ValidDisplayOrder, "Original description").Value;
        }
    }
}
