namespace Application.Services.Interfaces;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}