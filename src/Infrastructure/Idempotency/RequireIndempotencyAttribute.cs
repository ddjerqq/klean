namespace Infrastructure.Idempotency;

/// <summary>
/// Identifies an action that should be idempotent.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireIdempotencyAttribute : Attribute;