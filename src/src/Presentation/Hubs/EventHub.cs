using Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class EventHub : Hub<IEventHubClient>, IEventBroadcast
{
    public async Task BroadcastEvent(string name, string content)
    {
        await Clients.All.ReceiveEvent(name, content);
    }
}