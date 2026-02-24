using MasarHub.Application.Common.Results.Errors;

namespace MasarHub.Application.Common.Results
{
    public class Result
    {
        private readonly List<Error> _errors = new();
        public IReadOnlyList<Error> Errors => _errors.AsReadOnly();

        public bool IsFailure => _errors.Any();
        public bool IsSuccess => !_errors.Any();
        public string? Message { get; }

        protected Result() { }
        protected Result(string message)
        {
            Message = message;
        }
        protected Result(Error error)
        {
            _errors.Add(error);
        }
        protected Result(List<Error> errors)
        {
            _errors.AddRange(errors);
        }


        public static Result Success() => new Result();
        public static Result Success(string message) => new Result(message);
        public static Result Fail(Error error) => new Result(error);
        public static Result Fail(List<Error> errors) => new Result(errors);

        // Implicit conversions
        public static implicit operator Result(string message) => Success(message);
        public static implicit operator Result(Error error) => Fail(error);
        public static implicit operator Result(List<Error> errors) => Fail(errors);
    }
}