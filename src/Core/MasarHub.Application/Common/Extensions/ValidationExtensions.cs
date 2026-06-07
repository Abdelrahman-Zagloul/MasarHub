using FluentValidation;

namespace MasarHub.Application.Common.Extensions;

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

    public static IRuleBuilderOptions<T, string?> MaxLengthValidation<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength,
        string propertyName)
    {
        return ruleBuilder
            .Must(value => value == null || value.Length <= maxLength)
            .WithErrorCode("validation.max_length")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, string?> MinLengthValidation<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int minLength,
        string propertyName)
    {
        return ruleBuilder
            .Must(value => value == null || value.Length >= minLength)
            .WithErrorCode("validation.min_length")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, string?> LengthValidation<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int length,
        string propertyName)
    {
        return ruleBuilder
            .Must(value => value == null || value.Length == length)
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

    public static IRuleBuilderOptions<T, TProperty?> ValidEnum<T, TProperty>(
     this IRuleBuilder<T, TProperty?> ruleBuilder,
     string propertyName) where TProperty : struct, Enum
    {
        return ruleBuilder
            .IsInEnum()
            .WithErrorCode("validation.invalid_enum")
            .WithName(propertyName)
            .When(x => x != null);
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

    public static IRuleBuilderOptions<T, Guid> ValidGuid<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        string propertyName)
    {
        return ruleBuilder
            .NotEmpty()
            .WithErrorCode("validation.invalid_guid")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, Guid?> ValidNullableGuid<T>(
    this IRuleBuilder<T, Guid?> ruleBuilder,
    string propertyName)
    {
        return ruleBuilder
            .Must(guid => guid == null || guid != Guid.Empty)
            .WithErrorCode("validation.invalid_guid")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, string> ValidOtpCode<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string propertyName)
    {
        return ruleBuilder
            .Matches(@"^\d{6}$")
            .WithErrorCode("validation.invalid_otp")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, decimal> ValidPrice<T>(
        this IRuleBuilder<T, decimal> ruleBuilder,
        string propertyName, decimal minValue)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(minValue)
            .WithErrorCode("validation.invalid_price")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, IEnumerable<TElement>> RequiredCollection<T, TElement>(
        this IRuleBuilder<T, IEnumerable<TElement>> ruleBuilder,
        string propertyName)
    {
        return ruleBuilder
            .NotNull()
            .WithErrorCode("validation.required")
            .WithName(propertyName);
    }

    public static IRuleBuilderOptions<T, IEnumerable<TElement>> RequiredNonEmptyCollection<T, TElement>(
        this IRuleBuilder<T, IEnumerable<TElement>> ruleBuilder,
        string propertyName)
    {
        return ruleBuilder
            .NotEmpty()
            .WithErrorCode("validation.required")
            .WithName(propertyName);
    }
}