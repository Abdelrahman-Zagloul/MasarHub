using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Courses.Lessons;

namespace MasarHub.Domain.UnitTests.Modules.Courses.Lessons
{
    public sealed class LessonAttachmentTests
    {
        private static readonly Guid LessonId = Guid.NewGuid();
        private const string PublicId = "attachment_pub_123";
        private const string FileName = "document.pdf";
        private const string ContentType = "application/pdf";
        private const long FileSizeInBytes = 2048;

        #region Create

        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            var result = LessonAttachment.Create(LessonId, PublicId, FileName, ContentType, FileSizeInBytes);

            Assert.True(result.IsSuccess);
            Assert.Equal(LessonId, result.Value.LessonId);
            Assert.Equal(PublicId, result.Value.PublicId);
            Assert.Equal(FileName, result.Value.FileName);
            Assert.Equal(ContentType, result.Value.FileType);
            Assert.Equal(FileSizeInBytes, result.Value.FileSizeInBytes);
        }

        [Fact]
        public void Create_EmptyLessonId_ReturnsError()
        {
            var result = LessonAttachment.Create(Guid.Empty, PublicId, FileName, ContentType, FileSizeInBytes);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("lessonId").Code, result.Error.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidPublicId_ReturnsError(string? publicId)
        {
            var result = LessonAttachment.Create(LessonId, publicId!, FileName, ContentType, FileSizeInBytes);

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidFileName_ReturnsError(string? fileName)
        {
            var result = LessonAttachment.Create(LessonId, PublicId, fileName!, ContentType, FileSizeInBytes);

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidContentType_ReturnsError(string? contentType)
        {
            var result = LessonAttachment.Create(LessonId, PublicId, FileName, contentType!, FileSizeInBytes);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Create_NegativeFileSize_ReturnsError()
        {
            var result = LessonAttachment.Create(LessonId, PublicId, FileName, ContentType, -1);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region CanAddMoreAttachment

        [Fact]
        public void CanAddMoreAttachment_CountBelowMax_ReturnsSuccess()
        {
            var result = LessonAttachment.CanAddMoreAttachment(5);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void CanAddMoreAttachment_CountAtMax_ReturnsSuccess()
        {
            var result = LessonAttachment.CanAddMoreAttachment(10);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void CanAddMoreAttachment_CountExceedsMax_ReturnsError()
        {
            var result = LessonAttachment.CanAddMoreAttachment(11);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region UpdateFileName

        [Fact]
        public void UpdateFileName_ValidInput_ReturnsSuccess()
        {
            var attachment = CreateValidAttachment();
            var newFileName = "updated_document.pdf";

            var result = attachment.UpdateFileName(newFileName);

            Assert.True(result.IsSuccess);
            Assert.Equal(newFileName, attachment.FileName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateFileName_InvalidInput_ReturnsError(string? fileName)
        {
            var attachment = CreateValidAttachment();

            var result = attachment.UpdateFileName(fileName!);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_ReturnsSuccess()
        {
            var attachment = CreateValidAttachment();

            var result = attachment.Delete();

            Assert.True(result.IsSuccess);
            Assert.True(attachment.IsDeleted);
            Assert.NotNull(attachment.DeletedAt);
            Assert.Single(attachment.DomainEvents);
        }

        #endregion

        private static LessonAttachment CreateValidAttachment()
        {
            return LessonAttachment.Create(LessonId, PublicId, FileName, ContentType, FileSizeInBytes).Value;
        }
    }
}
