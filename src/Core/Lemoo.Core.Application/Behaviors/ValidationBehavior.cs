using FluentValidation;
using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;
using Lemoo.Core.Common.Errors;
using MediatR;

namespace Lemoo.Core.Application.Behaviors;

/// <summary>
/// Validation exception
/// </summary>
public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Validation failed")
    {
        Errors = errors;
    }
}

/// <summary>
/// Validation pipeline behavior that returns Result<T> instead of throwing exceptions
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
    where TRequest : MediatR.IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // If no validators exist, proceed to next handler
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            // Build error dictionary
            var errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());

            // Build error messages list
            var errorMessages = errors.SelectMany(kvp =>
                kvp.Value.Select(msg => $"{kvp.Key}: {msg}")).ToList();

            // Create failure result
            var failureResult = Result.Failure(errorMessages);

            // Handle different response types
            return HandleValidationFailure(failureResult, errors);
        }

        return await next();
    }

    private static TResponse HandleValidationFailure(Result failureResult, Dictionary<string, string[]> errors)
    {
        // If TResponse is Result or Result<T>, return the failure directly
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)failureResult;
        }

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            // Create Result<T> with failure
            var resultType = typeof(Result<>).MakeGenericType(typeof(TResponse).GetGenericArguments()[0]);
            var failureMethodInfo = resultType.GetMethod(nameof(Result.Failure), new[] { typeof(IEnumerable<string>) });

            if (failureMethodInfo != null)
            {
                var failure = failureMethodInfo.Invoke(null, new object[] { failureResult.Errors });
                return (TResponse)failure!;
            }
        }

        // For non-Result response types, throw exception as before
        // This maintains backward compatibility for commands that don't use Result pattern
        throw new ValidationException(errors);
    }
}
