namespace WebAPI.Hubs;

/// <summary>
/// a client for the event hub
/// </summary>
public interface IEventHubClient
{
    /// <summary>
    /// receive an event
    /// </summary>
    public Task ReceiveEvent(string name, string content);
}