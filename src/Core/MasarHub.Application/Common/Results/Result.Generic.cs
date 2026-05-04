using MasarHub.Application.Common.Results.Errors;

namespace MasarHub.Application.Common.Results
{
    public class Result<TValue> : Result
    {
        private readonly TValue _value;

        public TValue Value => IsSuccess
            ? _value
            : throw new InvalidOperationException("Cannot access value of a failed result.");

        private Result(TValue value)
        {
            _value = value;
        }

        private Result(Error error) : base(error)
        {
            _value = default!;
        }

        private Result(IEnumerable<Error> errors) : base(errors)
        {
            _value = default!;
        }

        public static Result<TValue> Success(TValue value)
            => new(value);

        public new static Result<TValue> Failure(Error error)
            => new(error);

        public new static Result<TValue> Failure(IEnumerable<Error> errors)
            => new(errors);

        // Implicit conversions
        public static implicit operator Result<TValue>(TValue value)
            => Success(value);

        public static implicit operator Result<TValue>(Error error)
            => Failure(error);

        public static implicit operator Result<TValue>(List<Error> errors)
            => Failure(errors);
    }
}