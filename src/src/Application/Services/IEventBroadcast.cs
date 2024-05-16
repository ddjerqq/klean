namespace Application.Services;

public interface IEventBroadcast
{
    public Task BroadcastEvent(string name, string content);
}