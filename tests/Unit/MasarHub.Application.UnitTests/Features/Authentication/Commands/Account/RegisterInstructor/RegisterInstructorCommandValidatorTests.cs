
using FluentAssertions;
using MasarHub.Application.Features.Authentication.Commands.Account.RegisterInstructor;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.UnitTests.Features.Authentication.Commands.Account.RegisterInstructor
{
    [Trait("UnitTests", "Feature.Auth.RegisterInstructor.Validator")]
    public sealed class RegisterInstructorCommandValidatorTests
    {
        private readonly RegisterInstructorCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new RegisterInstructorCommand(
                "Abdelrahman", "abdelrahman@example.com", "01009876543",
                Gender.Male, "SecurePass456!", "Software Engineer", null, null, []);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_InvalidFullName_ReturnsValidationError(string fullName)
        {
            var command = new RegisterInstructorCommand(
                fullName, "abdelrahman@example.com", "01009876543",
                Gender.Male, "SecurePass456!", "Engineer", null, null, []);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "FullName");
        }

        [Theory]
        [InlineData("")]
        [InlineData("address")]
        public void Validate_InvalidEmail_ReturnsValidationError(string email)
        {
            var command = new RegisterInstructorCommand(
                "Abdelrahman", email, "01009876543",
                Gender.Male, "SecurePass456!", "Engineer", null, null, []);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Theory]
        [InlineData("")]
        [InlineData("1234567890")]
        public void Validate_InvalidPhoneNumber_ReturnsValidationError(string phone)
        {
            var command = new RegisterInstructorCommand(
                "Abdelrahman", "abdelrahman@example.com", phone,
                Gender.Male, "SecurePass456!", "Engineer", null, null, []);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PhoneNumber");
        }

        [Theory]
        [InlineData("")]
        [InlineData("weak")]
        public void Validate_InvalidPassword_ReturnsValidationError(string password)
        {
            var command = new RegisterInstructorCommand(
                "Abdelrahman", "abdelrahman@example.com", "01009876543",
                Gender.Male, password, "Engineer", null, null, []);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_InvalidHeadline_ReturnsValidationError(string headline)
        {
            var command = new RegisterInstructorCommand(
                "Abdelrahman", "abdelrahman@example.com", "01009876543",
                Gender.Male, "SecurePass456!", headline, null, null, []);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Headline");
        }

        [Fact]
        public void Validate_HeadlineExceedsMaxLength_ReturnsValidationError()
        {
            var longHeadline = new string('A', 101);
            var command = new RegisterInstructorCommand(
                "Abdelrahman", "abdelrahman@example.com", "01009876543",
                Gender.Male, "SecurePass456!", longHeadline, null, null, []);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Headline");
        }

        [Fact]
        public void Validate_TooManySocialLinks_ReturnsValidationError()
        {
            var links = Enumerable.Range(1, 11).Select(i =>
                new SocialLinkRequest($"Platform{i}", $"https://platform{i}.com")).ToList();
            var command = new RegisterInstructorCommand(
                "Abdelrahman", "abdelrahman@example.com", "01009876543",
                Gender.Male, "SecurePass456!", "Engineer", null, null, links);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.social_links_limit");
        }

        [Theory]
        [InlineData("", "https://validurl.com", "PlatformName")]
        [InlineData("GitHub", "not-a-valid-url", "Url")]
        public void Validate_InvalidSocialLinkDetails_ReturnsValidationError(string platform, string url, string expectedErrorProperty)
        {
            var invalidLink = new SocialLinkRequest(platform, url);
            var command = new RegisterInstructorCommand(
                "Abdelrahman", "abdelrahman@example.com", "01009876543",
                Gender.Male, "SecurePass456!", "Engineer", null, null, [invalidLink]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains(expectedErrorProperty));
        }
    }
}