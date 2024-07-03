using Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

/// <inheritdoc cref="Hub{T}"/>c />
public class EventHub : Hub<IEventHubClient>, IEventBroadcast
{
    /// <inheritdoc />
    public async Task BroadcastEvent(string name, string content)
    {
        await Clients.All.ReceiveEvent(name, content);
    }
}