using FluentValidation;

namespace MasarHub.Application.Common.Validation;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string> Required<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string propertyName)
    {
        return ruleBuilder
            .NotEmpty()
            .WithErrorCode("validation.required")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, string> MaxLengthValidation<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int maxLength,
        string propertyName)
    {
        return ruleBuilder
            .MaximumLength(maxLength)
            .WithErrorCode("validation.max_length")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, string> MinLengthValidation<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int minLength,
        string propertyName)
    {
        return ruleBuilder
            .MinimumLength(minLength)
            .WithErrorCode("validation.min_length")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, string> LengthValidation<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int length,
        string propertyName)
    {
        return ruleBuilder
            .Length(length)
            .WithErrorCode("validation.length")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, string> ValidEmail<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string propertyName)
    {
        return ruleBuilder
            .EmailAddress()
            .WithErrorCode("validation.invalid_email")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, TProperty> ValidEnum<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder,
        string propertyName) where TProperty : struct, Enum
    {
        return ruleBuilder
            .IsInEnum()
            .WithErrorCode("validation.invalid_enum")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, string> ValidPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string propertyName)
    {
        return ruleBuilder
            .Required(propertyName)
            .MinLengthValidation(8, propertyName)
            .Matches("[0-9]")
                .WithErrorCode("validation.password_requires_number")
                .WithName(propertyName)
            .Matches("[a-z]")
                .WithErrorCode("validation.password_requires_lowercase")
                .WithName(propertyName)
            .Matches("[A-Z]")
                .WithErrorCode("validation.password_requires_uppercase")
                .WithName(propertyName); ;
    }

    public static IRuleBuilderOptions<T, string> ValidUrl<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string propertyName)
    {
        return ruleBuilder
            .Required(propertyName)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var result)
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
            .WithErrorCode("validation.invalid_url")
            .WithName(propertyName);
    }
}