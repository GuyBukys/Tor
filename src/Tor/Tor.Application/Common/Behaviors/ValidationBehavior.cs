using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Tor.Application.Common.Errors.ErrorTypes;

namespace Tor.Application.Common.Behaviors;

internal class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResultBase
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
    private readonly IValidator<TRequest>? _validator;

    public ValidationBehavior(
        ILogger<ValidationBehavior<TRequest, TResponse>> logger,
        IValidator<TRequest>? validator = null)
    {
        _logger = logger;
        _validator = validator;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator is null)
        {
            return await next();
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid)
        {
            return await next();
        }

        var errors = validationResult.Errors
            .ConvertAll(x => new ValidationError(x.PropertyName, x.ErrorMessage));

        _logger.LogError("failed validation for request {@request}. validation errors: {errors}", request, errors);

        return (dynamic)Result.Fail(errors);
    }
}
