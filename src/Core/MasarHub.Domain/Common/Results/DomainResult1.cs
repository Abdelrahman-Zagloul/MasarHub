using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Common.Results
{
    public class DomainResult<T> : DomainResult
    {
        public T Value { get; }

        private DomainResult(T value) : base(true, DomainError.None)
        {
            Value = value;
        }
        private DomainResult(DomainError error) : base(false, error)
        {
            Value = default!;
        }


        public static DomainResult<T> Success(T value) => new(value);
        public static new DomainResult<T> Failure(DomainError error) => new(error);


        public static implicit operator DomainResult<T>(T value) => Success(value);
        public static implicit operator DomainResult<T>(DomainError error) => Failure(error);
    }
}
