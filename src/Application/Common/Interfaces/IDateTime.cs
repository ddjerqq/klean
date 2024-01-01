namespace Application.Common.Interfaces;

/// <summary>
/// An interface for getting the current date and time.
/// This exists for testing purposes.
/// </summary>
public interface IDateTime
{
    /// <summary>
    /// Gets the current date and time in UTC.
    /// </summary>
    public DateTime UtcNow { get; }
}