using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Common.Results
{
    public class Result<T> : Result
    {
        public T Value { get; }

        private Result(T value) : base(true, DomainError.None)
        {
            Value = value;
        }
        private Result(DomainError error) : base(false, error)
        {
            Value = default!;
        }


        public static Result<T> Success(T value) => new(value);
        public static new Result<T> Failure(DomainError error) => new(error);


        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(DomainError error) => Failure(error);
    }
}
