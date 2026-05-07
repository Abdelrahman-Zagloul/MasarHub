using MasarHub.Application.Common.Results.Errors;

namespace MasarHub.Application.Common.Results
{
    public class Result
    {
        private readonly List<Error> _errors = new();
        public IReadOnlyList<Error> Errors => _errors.AsReadOnly();
        public string? SuccessCode { get; protected set; }
        public bool IsFailure => _errors.Any();
        public bool IsSuccess => !_errors.Any();

        protected Result() { }
        protected Result(string successCode)
        {
            SuccessCode = successCode;
        }
        protected Result(Error error)
        {
            _errors.Add(error);
        }

        protected Result(IEnumerable<Error> errors)
        {
            _errors.AddRange(errors);
        }

        public static Result Success() => new();
        public static Result Success(string successCode) => new Result(successCode);
        public static Result Failure(Error error) => new(error);
        public static Result Failure(IEnumerable<Error> errors) => new(errors);

        // Implicit conversions
        public static implicit operator Result(Error error) => Failure(error);

        public static implicit operator Result(List<Error> errors) => Failure(errors);
    }
}