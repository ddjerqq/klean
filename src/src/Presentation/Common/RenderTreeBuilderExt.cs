#pragma warning disable ASP0006
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;

namespace Presentation.Common;

public static class RenderTreeBuilderExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddAttributeIfNotNullOrWhiteSpace(this RenderTreeBuilder builder, int sequence, string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddAttribute(sequence, name, value);
    }
}
