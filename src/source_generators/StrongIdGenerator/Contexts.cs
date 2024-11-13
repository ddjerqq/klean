using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace StrongIdGenerator;

internal readonly record struct EntityStrongIdContext(string? Namespace, string TypeName)
{
    public static EntityStrongIdContext FromEntityTypeInfo(INamedTypeSymbol entityType, CancellationToken ct)
    {
        var ns = entityType.ContainingNamespace.IsGlobalNamespace
            ? null
            : entityType.ContainingNamespace.ToDisplayString();

        var name = entityType.Name;

        return new EntityStrongIdContext(ns, name);
    }
}

internal sealed class PartialClassContextEqualityComparer : EqualityComparer<EntityStrongIdContext>
{
    private PartialClassContextEqualityComparer()
    {
    }

    public static IEqualityComparer<EntityStrongIdContext> Instance { get; } = new PartialClassContextEqualityComparer();

    public override bool Equals(EntityStrongIdContext x, EntityStrongIdContext y)
    {
        return x.Namespace == y.Namespace
               && x.TypeName == y.TypeName;
    }

    public override int GetHashCode(EntityStrongIdContext obj)
    {
        return HashCode.Combine(obj.Namespace, obj.TypeName);
    }
}