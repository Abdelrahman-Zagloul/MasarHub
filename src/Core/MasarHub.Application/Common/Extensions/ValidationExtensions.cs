using FluentValidation;

namespace MasarHub.Application.Common.Extensions
{
    public static class ValidationExtensions
    {
        #region Required
        public static IRuleBuilderOptions<T, string> Required<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            string propertyName)
        {
            return ApplyValidation(
                ruleBuilder.NotEmpty(),
                "validation.required",
                propertyName);
        }
        public static IRuleBuilderOptions<T, TProperty> RequiredCollection<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            string propertyName)
        {
            return ApplyValidation(
                ruleBuilder.NotNull(),
                "validation.required",
                propertyName);
        }
        public static IRuleBuilderOptions<T, TProperty> RequiredNonEmptyCollection<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            string propertyName)
        {
            return ApplyValidation(
                ruleBuilder.NotEmpty(),
                "validation.required",
                propertyName);
        }
        #endregion

        #region String
        public static IRuleBuilderOptions<T, string?> ValidMaxLength<T>(
            this IRuleBuilder<T, string?> ruleBuilder,
            int maxLength,
            string propertyName)
        {
            return ApplyValidation(
                ruleBuilder.MaximumLength(maxLength),
                "validation.max_length",
                propertyName);
        }
        public static IRuleBuilderOptions<T, string?> ValidMinLength<T>(
            this IRuleBuilder<T, string?> ruleBuilder,
            int minLength,
            string propertyName)
        {
            return ApplyValidation(
                ruleBuilder.MinimumLength(minLength),
                "validation.min_length",
                propertyName);
        }
        public static IRuleBuilderOptions<T, string?> ValidLength<T>(
            this IRuleBuilder<T, string?> ruleBuilder,
            int length,
            string propertyName)
        {
            return ApplyValidation(
                ruleBuilder.Length(length),
                "validation.length",
                propertyName);
        }

        #endregion

        #region Numeric
        public static IRuleBuilderOptions<T, TProperty> ValidGreaterThanZero<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            string propertyName)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ApplyValidation(
                ruleBuilder.GreaterThan(default(TProperty)),
                "validation.greater_than_zero",
                propertyName);
        }
        public static IRuleBuilderOptions<T, TProperty?> ValidGreaterThanZero<T, TProperty>(
            this IRuleBuilder<T, TProperty?> ruleBuilder,
            string propertyName)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ApplyValidation(
                ruleBuilder.Must(v => !v.HasValue || v.Value.CompareTo(default(TProperty)) > 0),
                "validation.greater_than_zero",
                propertyName);
        }
        public static IRuleBuilderOptions<T, TProperty> ValidRange<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            TProperty min,
            TProperty max,
            string propertyName)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ApplyValidation(
                ruleBuilder.InclusiveBetween(min, max),
                "validation.invalid_range",
                propertyName);
        }
        public static IRuleBuilderOptions<T, TProperty?> ValidRange<T, TProperty>(
            this IRuleBuilder<T, TProperty?> ruleBuilder,
            TProperty min,
            TProperty max,
            string propertyName)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            var rule = ruleBuilder.Must((_, value, context) =>
            {
                context.MessageFormatter.AppendArgument("From", min);
                context.MessageFormatter.AppendArgument("To", max);
                return !value.HasValue || (value.Value.CompareTo(min) >= 0 && value.Value.CompareTo(max) <= 0);
            });

            return ApplyValidation(rule, "validation.invalid_range", propertyName);
        }

        #endregion

        #region Enum

        public static IRuleBuilderOptions<T, TProperty> ValidEnum<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            string propertyName)
            where TProperty : struct, Enum
        {
            return ApplyValidation(
                ruleBuilder.IsInEnum(),
                "validation.invalid_enum",
                propertyName);
        }

        public static IRuleBuilderOptions<T, TProperty?> ValidEnum<T, TProperty>(
            this IRuleBuilder<T, TProperty?> ruleBuilder,
            string propertyName)
            where TProperty : struct, Enum
        {
            return ApplyValidation(
                ruleBuilder.IsInEnum(),
                "validation.invalid_enum",
                propertyName);
        }

        #endregion

        #region Guid

        public static IRuleBuilderOptions<T, Guid> ValidGuid<T>(
            this IRuleBuilder<T, Guid> ruleBuilder,
            string propertyName)
        {
            return ApplyValidation(
                ruleBuilder.NotEmpty(),
                "validation.invalid_guid",
                propertyName);
        }

        public static IRuleBuilderOptions<T, Guid?> ValidGuid<T>(
            this IRuleBuilder<T, Guid?> ruleBuilder,
            string propertyName)
        {
            return ApplyValidation(
                ruleBuilder.Must(guid => !guid.HasValue || guid.Value != Guid.Empty),
                "validation.invalid_guid",
                propertyName);
        }

        #endregion

        #region Email & URL & Password & OTP
        public static IRuleBuilderOptions<T, string> ValidEmail<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            string propertyName)
        {
            return ApplyValidation(
                ruleBuilder.EmailAddress(),
                "validation.invalid_email",
                propertyName);
        }
        public static IRuleBuilderOptions<T, string> ValidUrl<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            string propertyName)
        {
            return ruleBuilder
                .Required(propertyName)
                .Must(url =>
                    Uri.TryCreate(url, UriKind.Absolute, out var result) &&
                    (result.Scheme == Uri.UriSchemeHttp ||
                     result.Scheme == Uri.UriSchemeHttps))
                .WithErrorCode("validation.invalid_url")
                .WithName(propertyName);
        }
        public static IRuleBuilderOptions<T, string> ValidPassword<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            string propertyName)
        {
            return ruleBuilder
                .Required(propertyName)
                .ValidMinLength(8, propertyName)
                .Matches("[0-9]")
                    .WithErrorCode("validation.password_requires_number")
                .Matches("[a-z]")
                    .WithErrorCode("validation.password_requires_lowercase")
                .Matches("[A-Z]")
                    .WithErrorCode("validation.password_requires_uppercase")
                .WithName(propertyName);
        }
        public static IRuleBuilderOptions<T, string> ValidOtpCode<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            string propertyName)
        {
            return ApplyValidation(
                ruleBuilder.Matches(@"^\d{6}$"),
                "validation.invalid_otp",
                propertyName);
        }

        #endregion

        #region Helper 
        private static IRuleBuilderOptions<T, TProperty> ApplyValidation<T, TProperty>(
            IRuleBuilderOptions<T, TProperty> rule,
            string errorCode,
            string propertyName)
        {
            return rule
                .WithErrorCode(errorCode)
                .WithName(propertyName);
        }
        #endregion
    }
}