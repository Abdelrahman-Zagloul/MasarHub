

using MasarHub.Domain.SharedKernel.Exceptions;

namespace MasarHub.Domain.SharedKernel
{
    public static class Guard
    {
        public static T AgainstNull<T>(T? value, string propertyName) where T : class
        {
            if (value is null)
                throw new DomainException(ErrorCodes.General.Null, propertyName);

            return value;
        }

        public static string AgainstNullOrWhiteSpace(string? value, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(ErrorCodes.General.NullOrEmpty, propertyName);

            return value;
        }

        public static T AgainstNegative<T>(T value, string propertyName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default(T)) < 0)
                throw new DomainException(ErrorCodes.General.Negative, propertyName);

            return value;
        }

        public static T AgainstNegativeOrZero<T>(T value, string propertyName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default(T)) <= 0)
                throw new DomainException(ErrorCodes.General.NegativeOrZero, propertyName);

            return value;
        }

        public static Guid AgainstEmptyGuid(Guid value, string propertyName)
        {
            if (value == Guid.Empty)
                throw new DomainException(ErrorCodes.General.EmptyGuid, propertyName);

            return value;
        }
        public static T AgainstEnumOutOfRange<T>(T value, string propertyName) where T : struct, Enum
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw new DomainException(ErrorCodes.General.InvalidEnum, propertyName);

            return value;
        }
    }
}