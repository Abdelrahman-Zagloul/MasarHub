using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Common.Guards
{
    public static class Guard
    {
        public static DomainError? AgainstNull<T>(T? value, string propertyName) where T : class
        {
            if (value is null)
                return DomainError.Null(propertyName);

            return null;
        }
        public static DomainError? AgainstNullOrWhiteSpace(string? value, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DomainError.NullOrEmpty(propertyName);

            return null;
        }
        public static DomainError? AgainstNegative<T>(T value, string propertyName)
            where T : struct, IComparable<T>
        {
            if (value.CompareTo(default) < 0)
                return DomainError.Negative(propertyName);

            return null;
        }
        public static DomainError? AgainstNegativeOrZero<T>(T value, string propertyName)
            where T : struct, IComparable<T>
        {
            if (value.CompareTo(default) <= 0)
                return DomainError.NegativeOrZero(propertyName);

            return null;
        }
        public static DomainError? AgainstEmptyGuid(Guid value, string propertyName)
        {
            if (value == Guid.Empty)
                return DomainError.EmptyGuid(propertyName);

            return null;
        }
        public static DomainError? AgainstEnumOutOfRange<T>(T value, string propertyName)
            where T : struct, Enum
        {
            if (!Enum.IsDefined(typeof(T), value))
                return DomainError.InvalidEnum(propertyName);

            return null;
        }
        public static DomainError? AgainstInvalidUrl(string value, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DomainError.NullOrEmpty(propertyName);

            if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                return DomainError.InvalidUrl(propertyName);

            return null;
        }
    }
}
