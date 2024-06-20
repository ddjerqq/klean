using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace StrongIdGenerator;

internal readonly record struct EntityStrongIdContext(string? Namespace, string TypeName, string IdType)
{
    public static EntityStrongIdContext FromEntityTypeInfo((INamedTypeSymbol EntityType, INamedTypeSymbol IdType)? typeInfo, CancellationToken ct)
    {
        var type = typeInfo!.Value;
        var ns = type.EntityType.ContainingNamespace.IsGlobalNamespace
            ? null
            : type.EntityType.ContainingNamespace.ToDisplayString();

        var name = type.EntityType.Name;
        var targetType = type.IdType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return new EntityStrongIdContext(ns, name, targetType);
    }
}

internal sealed class PartialClassContextEqualityComparer : EqualityComparer<EntityStrongIdContext>
{
    private PartialClassContextEqualityComparer() {}
    public static IEqualityComparer<EntityStrongIdContext> Instance { get; } = new PartialClassContextEqualityComparer();

    public override bool Equals(EntityStrongIdContext x, EntityStrongIdContext y) =>
        x.Namespace == y.Namespace
        && x.TypeName == y.TypeName
        && x.IdType == y.IdType;

    public override int GetHashCode(EntityStrongIdContext obj) => HashCode.Combine(obj.Namespace, obj.TypeName, obj.IdType);
}