namespace Application.Services;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}