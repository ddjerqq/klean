using Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

/// <inheritdoc />
public class EventBroadcast(IHubContext<EventHub, IEventHubClient> hub) : IEventBroadcast
{
    /// <inheritdoc />
    public Task BroadcastEvent(string name, string content)
    {
        return hub.Clients.All.ReceiveEvent(name, content);
    }
}