using Application.Services;
using MediatR;
using Serilog;
using Serilog.Events;
using SerilogTracing;

namespace Application.Behaviours;

internal sealed class LoggingBehaviour<TRequest, TResponse>(ICurrentUserAccessor currentUser) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var userId = currentUser.Id?.ToString() ?? "unauthenticated";

        using var activity = Log.Logger.StartActivity("{@UserId} sent request {@RequestName} {@Request}", userId, typeof(TRequest).Name, request);

        try
        {
            var result = await next();
            activity.AddProperty("Result", result, true);
            activity.Complete();
            return result;
        }
        catch (Exception ex)
        {
            activity.Complete(LogEventLevel.Fatal, ex);
            throw;
        }
    }
}