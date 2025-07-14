using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                foreach (var failure in failures)
                {
                    _logger.LogWarning(
                        "Validation Failure: Property {PropertyName}, Error: {ErrorMessage}",
                        failure.PropertyName,
                        failure.ErrorMessage
                    );
                }

                throw new ValidationException(failures);
            }
        }
        return await next();
    }
}