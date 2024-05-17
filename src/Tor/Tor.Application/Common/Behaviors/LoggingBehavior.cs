using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Tor.Application.Common.Errors.ErrorTypes;

namespace Tor.Application.Common.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResultBase
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("started handling request {@Request}", request);

            var result = await next();

            if (result.IsFailed)
            {
                _logger.LogError("Failed result for request {@Request}. Reason: {Reason}",
                    request,
                    result.Errors.FirstOrDefault()?.Message);
            }

            _logger.LogInformation("finished handling request {@Request}", request);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while handling request {@Request}", request);
            return (dynamic)Result.Fail(new UnknownError(ex.Message));
        }
    }
}
