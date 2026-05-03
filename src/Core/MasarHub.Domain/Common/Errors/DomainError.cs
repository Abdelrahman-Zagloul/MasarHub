namespace MasarHub.Domain.Common.Errors
{
    public sealed record DomainError(string Code, string? PropertyName = null)
    {
        public static DomainError None => new("");

        public static DomainError Null(string? propertyName = null) => new($"{propertyName}.null", propertyName);
        public static DomainError Empty(string? propertyName = null) => new($"{propertyName}.empty", propertyName);
        public static DomainError NullOrEmpty(string? propertyName = null) => new($"{propertyName}.null_or_empty", propertyName);
        public static DomainError Invalid(string? propertyName = null) => new($"{propertyName}.invalid", propertyName);
        public static DomainError Negative(string? propertyName = null) => new($"{propertyName}.negative", propertyName);
        public static DomainError NegativeOrZero(string? propertyName = null) => new($"{propertyName}.negative_or_zero", propertyName);
        public static DomainError EmptyGuid(string? propertyName = null) => new($"{propertyName}.empty_guid", propertyName);
        public static DomainError InvalidEnum(string? propertyName = null) => new($"{propertyName}.invalid_enum", propertyName);
        public static DomainError AlreadyDeleted(string? propertyName = null) => new($"{propertyName}.already_deleted", propertyName);
        public static DomainError NotDeleted(string? propertyName = null) => new($"{propertyName}.not_deleted", propertyName);
        public static DomainError InvalidUrl(string? propertyName = null) => new($"{propertyName}.invalid_url", propertyName);
        public static DomainError TooManyItems(string? propertyName = null) => new($"{propertyName}.too_many_items", propertyName);
        public static DomainError Duplicate(string? propertyName = null) => new($"{propertyName}.duplicate", propertyName);
    };
}
