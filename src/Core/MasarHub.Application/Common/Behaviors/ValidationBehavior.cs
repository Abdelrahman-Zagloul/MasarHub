using FluentValidation;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Common.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IResult

    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(x => x.Errors)
                .Where(x => x is not null)
                .ToList();

            if (!failures.Any())
                return await next();

            var errors = failures.Select(f =>
                Error.Validation(f.ErrorCode, f.PropertyName,
                       f.FormattedMessagePlaceholderValues.ToDictionary(x => x.Key, x => x.Value)!
            )).ToList();

            return CreateValidationResult(errors);
        }

        private static TResponse CreateValidationResult(List<Error> errors)
        {
            var responseType = typeof(TResponse);
            if (responseType == typeof(Result))
                return (TResponse)(object)Result.Failure(errors);


            var failResult = typeof(Result<>).MakeGenericType(responseType.GenericTypeArguments[0])
                  .GetMethod("Failure", [typeof(List<Error>)])!
                  .Invoke(null, [errors]);
            return (TResponse)failResult!;

        }
    }
}
