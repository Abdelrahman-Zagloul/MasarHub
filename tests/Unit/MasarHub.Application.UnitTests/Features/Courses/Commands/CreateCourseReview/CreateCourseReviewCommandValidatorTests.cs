using FluentAssertions;
using MasarHub.Application.Features.Courses.Commands.CreateCourseReview;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.CreateCourseReview
{
    [Trait("UnitTests.Feature.Courses", "CreateCourseReview")]
    public sealed class CreateCourseReviewCommandValidatorTests
    {
        private readonly CreateCourseReviewCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new CreateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), 4, "Good course");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var command = new CreateCourseReviewCommand(Guid.Empty, Guid.NewGuid(), 4, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_RatingBelow1_ReturnsError()
        {
            var command = new CreateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), 0, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.rating_invalid");
        }

        [Fact]
        public void Validate_RatingAbove5_ReturnsError()
        {
            var command = new CreateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), 6, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.rating_invalid");
        }

        [Fact]
        public void Validate_ReviewContentTooLong_ReturnsError()
        {
            var command = new CreateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), 3, new string('a', 2001));

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ReviewContent");
        }

        [Fact]
        public void Validate_ReviewContentNull_IsValid()
        {
            var command = new CreateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), 5, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
