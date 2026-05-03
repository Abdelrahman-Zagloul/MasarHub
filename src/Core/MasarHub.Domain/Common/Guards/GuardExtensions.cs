using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Common.Guards
{
    public static class GuardExtensions
    {
        public static DomainError? FirstError(params DomainError?[] errors)
            => errors.FirstOrDefault(e => e != null);
    }
}
