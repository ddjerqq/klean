namespace Infrastructure.Idempotency;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireIdempotencyAttribute : Attribute;