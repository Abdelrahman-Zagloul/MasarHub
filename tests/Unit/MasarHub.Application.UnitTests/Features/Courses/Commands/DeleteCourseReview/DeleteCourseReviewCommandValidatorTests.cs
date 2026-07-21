using FluentAssertions;
using FluentValidation.TestHelper;
using MasarHub.Application.Features.Courses.Commands.DeleteCourseReview;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.DeleteCourseReview
{
    [Trait("UnitTests.Feature.Courses", "DeleteCourseReview")]
    public sealed class DeleteCourseReviewCommandValidatorTests
    {
        private readonly DeleteCourseReviewCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new DeleteCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.TestValidate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsError()
        {
            var command = new DeleteCourseReviewCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyReviewId_ReturnsError()
        {
            var command = new DeleteCourseReviewCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

            var result = _sut.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ReviewId");
        }
    }
}
