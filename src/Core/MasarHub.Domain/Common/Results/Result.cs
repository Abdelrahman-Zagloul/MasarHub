using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Common.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public DomainError Error { get; }

        protected Result(bool isSuccess, DomainError error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, DomainError.None);
        public static Result Failure(DomainError error) => new(false, error);
        public static implicit operator Result(DomainError error) => Failure(error);
    }
}
