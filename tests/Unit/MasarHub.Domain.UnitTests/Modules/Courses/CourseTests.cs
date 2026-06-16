using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Courses.Events;

namespace MasarHub.Domain.UnitTests.Modules.Courses
{
    public sealed class CourseTests
    {
        private const string ValidTitle = "Advanced C# Programming";
        private const string ValidSlug = "advanced-csharp-programming";
        private const string ValidDescription = "Learn advanced C# concepts including delegates, LINQ, and async programming.";
        private const decimal ValidPrice = 99.99m;
        private const CourseLanguage ValidLanguage = CourseLanguage.English;
        private const CourseLevel ValidLevel = CourseLevel.Advanced;
        private static readonly Guid ValidInstructorId = Guid.NewGuid();
        private static readonly Guid ValidCategoryId = Guid.NewGuid();

        #region Create

        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            var result = Course.Create(ValidTitle, ValidSlug, ValidDescription, ValidPrice, ValidLanguage, ValidLevel, ValidInstructorId, ValidCategoryId);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(ValidTitle, result.Value.Title);
            Assert.Equal(ValidSlug, result.Value.Slug);
            Assert.Equal(ValidPrice, result.Value.Price);
            Assert.Equal(ValidLanguage, result.Value.Language);
            Assert.Equal(CourseStatus.Draft, result.Value.Status);
            Assert.Single(result.Value.DomainEvents);
            Assert.IsType<CourseCreatedDomainEvent>(result.Value.DomainEvents.First());
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidTitle_ReturnsError(string? title)
        {
            var result = Course.Create(title!, ValidSlug, ValidDescription, ValidPrice, ValidLanguage, ValidLevel, ValidInstructorId, ValidCategoryId);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.NullOrEmpty("title").Code, result.Error.Code);
        }

        [Fact]
        public void Create_InvalidPrice_ReturnsError()
        {
            var result = Course.Create(ValidTitle, ValidSlug, ValidDescription, -1, ValidLanguage, ValidLevel, ValidInstructorId, ValidCategoryId);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.Negative("price").Code, result.Error.Code);
        }

        [Fact]
        public void Create_EmptyInstructorId_ReturnsError()
        {
            var result = Course.Create(ValidTitle, ValidSlug, ValidDescription, ValidPrice, ValidLanguage, ValidLevel, Guid.Empty, ValidCategoryId);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("instructorId").Code, result.Error.Code);
        }

        [Fact]
        public void Create_EmptyCategoryId_ReturnsError()
        {
            var result = Course.Create(ValidTitle, ValidSlug, ValidDescription, ValidPrice, ValidLanguage, ValidLevel, ValidInstructorId, Guid.Empty);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("categoryId").Code, result.Error.Code);
        }

        [Fact]
        public void Create_InvalidLanguage_ReturnsError()
        {
            var result = Course.Create(ValidTitle, ValidSlug, ValidDescription, ValidPrice, (CourseLanguage)99, ValidLevel, ValidInstructorId, ValidCategoryId);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Create_InvalidLevel_ReturnsError()
        {
            var result = Course.Create(ValidTitle, ValidSlug, ValidDescription, ValidPrice, ValidLanguage, (CourseLevel)99, ValidInstructorId, ValidCategoryId);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region SubmitForApproval

        [Fact]
        public void SubmitForApproval_DraftStatus_ReturnsSuccess()
        {
            var course = CreateValidCourse();

            var result = course.SubmitForApproval();

            Assert.True(result.IsSuccess);
            Assert.Equal(CourseStatus.PendingApproval, course.Status);
            Assert.Contains(course.DomainEvents, e => e is CourseSubmittedForApprovalDomainEvent);
        }

        [Fact]
        public void SubmitForApproval_AlreadyPending_ReturnsAlreadySubmittedError()
        {
            var course = CreateValidCourse();
            course.SubmitForApproval();

            var result = course.SubmitForApproval();

            Assert.True(result.IsFailure);
            Assert.Equal(CourseErrors.AlreadySubmitted.Code, result.Error.Code);
        }

        [Fact]
        public void SubmitForApproval_AlreadyPublished_ReturnsAlreadyPublishedError()
        {
            var course = CreateValidCourse();
            course.SubmitForApproval();
            course.ApprovePublication(Guid.NewGuid());

            var result = course.SubmitForApproval();

            Assert.True(result.IsFailure);
            Assert.Equal(CourseErrors.AlreadyPublished.Code, result.Error.Code);
        }

        #endregion

        #region ApprovePublication

        [Fact]
        public void ApprovePublication_PendingStatus_ReturnsSuccess()
        {
            var course = CreateValidCourse();
            course.SubmitForApproval();
            var adminId = Guid.NewGuid();

            var result = course.ApprovePublication(adminId);

            Assert.True(result.IsSuccess);
            Assert.Equal(CourseStatus.Published, course.Status);
            Assert.NotNull(course.PublishedAt);
            Assert.Equal(adminId, course.ApprovedBy);
            Assert.Contains(course.DomainEvents, e => e is CourseApprovedDomainEvent);
        }

        [Fact]
        public void ApprovePublication_NotPending_ReturnsNotPendingApprovalError()
        {
            var course = CreateValidCourse();

            var result = course.ApprovePublication(Guid.NewGuid());

            Assert.True(result.IsFailure);
            Assert.Equal(CourseErrors.NotPendingApproval.Code, result.Error.Code);
        }

        [Fact]
        public void ApprovePublication_AlreadyPublished_ReturnsAlreadyPublishedError()
        {
            var course = CreateValidCourse();
            course.SubmitForApproval();
            course.ApprovePublication(Guid.NewGuid());

            var result = course.ApprovePublication(Guid.NewGuid());

            Assert.True(result.IsFailure);
            Assert.Equal(CourseErrors.AlreadyPublished.Code, result.Error.Code);
        }

        [Fact]
        public void ApprovePublication_EmptyAdminId_ReturnsError()
        {
            var course = CreateValidCourse();
            course.SubmitForApproval();

            var result = course.ApprovePublication(Guid.Empty);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region RejectPublication

        [Fact]
        public void RejectPublication_PendingStatus_ReturnsSuccess()
        {
            var course = CreateValidCourse();
            course.SubmitForApproval();
            var adminId = Guid.NewGuid();
            var reason = "Course content does not meet quality standards";

            var result = course.RejectPublication(reason, adminId);

            Assert.True(result.IsSuccess);
            Assert.Equal(CourseStatus.Rejected, course.Status);
            Assert.Equal(reason, course.RejectionReason);
            Assert.Equal(adminId, course.RejectedBy);
            Assert.Contains(course.DomainEvents, e => e is CourseRejectedDomainEvent);
        }

        [Fact]
        public void RejectPublication_AlreadyRejected_ReturnsAlreadyRejectedError()
        {
            var course = CreateValidCourse();
            course.SubmitForApproval();
            course.RejectPublication("First rejection", Guid.NewGuid());

            var result = course.RejectPublication("Second rejection", Guid.NewGuid());

            Assert.True(result.IsFailure);
            Assert.Equal(CourseErrors.AlreadyRejected.Code, result.Error.Code);
        }

        [Fact]
        public void RejectPublication_DraftStatus_ReturnsNotPendingApprovalError()
        {
            var course = CreateValidCourse();

            var result = course.RejectPublication("Some reason", Guid.NewGuid());

            Assert.True(result.IsFailure);
            Assert.Equal(CourseErrors.NotPendingApproval.Code, result.Error.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void RejectPublication_EmptyReason_ReturnsError(string? reason)
        {
            var course = CreateValidCourse();
            course.SubmitForApproval();

            var result = course.RejectPublication(reason!, Guid.NewGuid());

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void RejectPublication_EmptyAdminId_ReturnsError()
        {
            var course = CreateValidCourse();
            course.SubmitForApproval();

            var result = course.RejectPublication("Some reason", Guid.Empty);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region Update

        [Fact]
        public void UpdateTitle_ValidInput_ReturnsSuccess()
        {
            var course = CreateValidCourse();
            var newTitle = "Updated Course Title";

            var result = course.UpdateTitle(newTitle);

            Assert.True(result.IsSuccess);
            Assert.Equal(newTitle, course.Title);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateTitle_InvalidInput_ReturnsError(string? title)
        {
            var course = CreateValidCourse();

            var result = course.UpdateTitle(title!);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void UpdateDescription_ValidInput_ReturnsSuccess()
        {
            var course = CreateValidCourse();
            var newDescription = "Updated description";

            var result = course.UpdateDescription(newDescription);

            Assert.True(result.IsSuccess);
            Assert.Equal(newDescription, course.Description);
        }

        [Fact]
        public void UpdatePrice_ValidInput_ReturnsSuccess()
        {
            var course = CreateValidCourse();
            var newPrice = 149.99m;

            var result = course.UpdatePrice(newPrice);

            Assert.True(result.IsSuccess);
            Assert.Equal(newPrice, course.Price);
        }

        [Fact]
        public void UpdatePrice_NegativeValue_ReturnsError()
        {
            var course = CreateValidCourse();

            var result = course.UpdatePrice(-10);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Delete_ReturnsSuccess()
        {
            var course = CreateValidCourse();

            var result = course.Delete();

            Assert.True(result.IsSuccess);
            Assert.True(course.IsDeleted);
            Assert.NotNull(course.DeletedAt);
        }

        #endregion

        private static Course CreateValidCourse()
        {
            return Course.Create(ValidTitle, ValidSlug, ValidDescription, ValidPrice, ValidLanguage, ValidLevel, ValidInstructorId, ValidCategoryId).Value;
        }
    }
}
