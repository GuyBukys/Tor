using FluentResults;
using MediatR;

namespace Tor.Application.Common.Behaviors;

internal class UtcConverterBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResultBase
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        foreach (var prop in request.GetType().GetProperties())
        {
            if (prop.PropertyType != typeof(DateTime))
            {
                continue;
            }

            DateTime? propAsDateTime = (DateTime?)prop.GetValue(request);
            if (propAsDateTime is null)
            {
                continue;
            }

            if (propAsDateTime.Value.Kind != DateTimeKind.Utc)
            {
                prop.SetValue(request, propAsDateTime.Value.ToUniversalTime());
            }
        }

        return await next();
    }
}
