namespace MasarHub.Domain.SharedKernel.Exceptions
{
    public sealed class DomainException : Exception
    {
        public string Code { get; }
        public string? Property { get; }
        public DomainException(string code, string? property = null)
            : base(code)
        {
            Code = code;
            Property = property;
        }

        public DomainException(string code, string? property, Exception innerException)
            : base(code, innerException)
        {
            Code = code;
            Property = property;
        }
    }
}
