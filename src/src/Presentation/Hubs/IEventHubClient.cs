namespace Presentation.Hubs;

public interface IEventHubClient
{
    public Task ReceiveEvent(string name, string content);
}