using System.Diagnostics;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

internal sealed class LoggingBehaviour<TRequest, TResponse>(
    ICurrentUserAccessor currentUserAccessor,
    ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var user = await currentUserAccessor.GetCurrentUserAsync(ct);

        logger.LogInformation(
            "{@UserId} {@UserName} started request {@RequestName} {@Request}",
            user?.Id,
            user?.Username,
            typeof(TRequest).Name,
            request);

        var stopwatch = Stopwatch.StartNew();
        var result = await next();
        stopwatch.Stop();

        var end = stopwatch.ElapsedMilliseconds;

        logger.LogInformation(
            "{@UserId} {@UserName} finished request {@RequestName} {@Request} in {@Duration}ms",
            user?.Id,
            user?.Username,
            typeof(TRequest).Name,
            request,
            end);

        return result;
    }
}