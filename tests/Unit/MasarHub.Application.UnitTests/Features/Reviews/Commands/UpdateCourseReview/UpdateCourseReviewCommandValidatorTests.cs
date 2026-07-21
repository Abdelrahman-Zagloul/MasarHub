using FluentAssertions;
using FluentValidation.TestHelper;
using MasarHub.Application.Features.Reviews.Commands.UpdateCourseReview;

namespace MasarHub.Application.UnitTests.Features.Reviews.Commands.UpdateCourseReview
{
    [Trait("UnitTests.Feature.Courses", "UpdateCourseReview")]
    public sealed class UpdateCourseReviewCommandValidatorTests
    {
        private readonly UpdateCourseReviewCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new UpdateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 4, "Great course!");

            var result = _sut.TestValidate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var command = new UpdateCourseReviewCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), 4, null);

            var result = _sut.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyReviewId_ReturnsError()
        {
            var command = new UpdateCourseReviewCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), 4, null);

            var result = _sut.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ReviewId");
        }

        [Fact]
        public void Validate_RatingBelow1_ReturnsError()
        {
            var command = new UpdateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, null);

            var result = _sut.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.rating_invalid");
        }

        [Fact]
        public void Validate_RatingAbove5_ReturnsError()
        {
            var command = new UpdateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 6, null);

            var result = _sut.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.rating_invalid");
        }

        [Fact]
        public void Validate_ReviewContentTooLong_ReturnsError()
        {
            var command = new UpdateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, new string('a', 2001));

            var result = _sut.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ReviewContent");
        }

        [Fact]
        public void Validate_NoFieldsProvided_ReturnsError()
        {
            var command = new UpdateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, null);

            var result = _sut.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.at_least_one");
        }

        [Fact]
        public void Validate_RatingNull_ReviewContentProvided_IsValid()
        {
            var command = new UpdateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, "New content");

            var result = _sut.TestValidate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_RatingProvided_ReviewContentNull_IsValid()
        {
            var command = new UpdateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 3, null);

            var result = _sut.TestValidate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
