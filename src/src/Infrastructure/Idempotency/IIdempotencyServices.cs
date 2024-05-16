namespace Infrastructure.Idempotency;

public interface IIdempotencyService
{
    public bool ContainsKey(Guid key);

    public void AddKey(Guid key);
}