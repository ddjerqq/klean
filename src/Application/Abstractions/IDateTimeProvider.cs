namespace Application.Abstractions;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}