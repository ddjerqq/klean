namespace Application.Common;

public abstract record DiscriminatedUnionSample;

public sealed record String(string Value) : DiscriminatedUnionSample;

public sealed record Decimal(decimal Value) : DiscriminatedUnionSample;

public static class DiscriminatedUnionExt
{
    public static TResult Map<TResult>(this DiscriminatedUnionSample unionSample,
        Func<String, TResult> mapString,
        Func<Decimal, TResult> mapDecimal) => unionSample switch
    {
        String s => mapString(s),
        Decimal d => mapDecimal(d),
        _ => throw new InvalidOperationException("Unknown DiscriminatedUnion type"),
    };
}