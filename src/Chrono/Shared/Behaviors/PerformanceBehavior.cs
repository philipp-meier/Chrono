using System.Diagnostics;
using MediatR;

namespace Chrono.Shared.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
            requestName, elapsedMilliseconds, request);

        return response;
    }
}
