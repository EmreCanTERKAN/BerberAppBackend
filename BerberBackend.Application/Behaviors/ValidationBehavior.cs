using FluentValidation;
using FluentValidation.Results;
using MediatR;
using TS.Result;

namespace BerberApp_Backend.Application.Behaviors;
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = new List<FluentValidation.Results.ValidationResult>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            validationResults.Add(result);
        }

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            var errors = failures.Select(f => f.ErrorMessage).ToList();

            var resultType = typeof(TResponse);
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var innerType = resultType.GetGenericArguments()[0];
                var failureMethod = typeof(Result<>).MakeGenericType(innerType)
                    .GetMethod("Failure", new[] { typeof(int), typeof(List<string>) });

                var result = failureMethod?.Invoke(null, new object[] { 400, errors });
                return (TResponse)result!;
            }

            throw new ValidationException("Validation failed", failures);
        }

        return await next();
    }
}