using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Common.Results
{
    public class DomainResult
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public DomainError Error { get; }

        protected DomainResult(bool isSuccess, DomainError error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static DomainResult Success() => new(true, DomainError.None);
        public static DomainResult Failure(DomainError error) => new(false, error);
        public static implicit operator DomainResult(DomainError error) => Failure(error);
    }
}
