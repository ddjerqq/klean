using Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class EventBroadcast(IHubContext<EventHub, IEventHubClient> hub) : IEventBroadcast
{
    public Task BroadcastEvent(string name, string content)
    {
        return hub.Clients.All.ReceiveEvent(name, content);
    }
}