﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StrongIdGenerator;

[Generator]
public sealed class StrongIdGenerator : IIncrementalGenerator
{
    private const string GeneratedCodeAttribute = """[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Klean.StrongIdGenerator", "3.0.0")]""";

    private const string CommonSrc =
        $$"""
          // <auto-generated/>
          #pragma warning disable
          #nullable enable

          namespace Generated;

          {{GeneratedCodeAttribute}}
          public interface IStrongId;

          {{GeneratedCodeAttribute}}
          [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
          public sealed class StrongIdAttribute : global::System.Attribute;

          {{GeneratedCodeAttribute}}
          public static class SourceGeneratorExt
          {
              public static string ToSnakeCase(this string text)
              {
                  if (string.IsNullOrWhiteSpace(text))
                      throw new global::System.ArgumentNullException(nameof(text));
          
                  if (text.Length < 2)
                      return text;
          
                  var sb = new global::System.Text.StringBuilder();
                  sb.Append(char.ToLowerInvariant(text[0]));
          
                  for (var i = 1; i < text.Length; ++i)
                  {
                      var c = text[i];
                      if (char.IsUpper(c))
                      {
                          sb.Append('_');
                          sb.Append(char.ToLowerInvariant(c));
                      }
                      else
                      {
                          sb.Append(c);
                      }
                  }
          
                  return sb.ToString();
              }
          
              // public static global::Serilog.LoggerConfiguration Destructure_ByTransformingStrongIdsToStrings(this global::Serilog.LoggerConfiguration config)
              // {
              //     var types =
              //         from global::System.Type type in global::System.Reflection.Assembly.GetAssembly(typeof(global::Generated.StrongIdAttribute))!.GetTypes()
              //         where type.GetInterfaces().Any(i => i == typeof(global::Generated.IStrongId))
              //         select type;
              // 
              //     foreach (var type in types)
              //     {
              //         var destructure = config.Destructure;
              //         var method = typeof(global::Serilog.Configuration.LoggerDestructuringConfiguration).GetMethod(nameof(global::Serilog.Configuration.LoggerDestructuringConfiguration.ByTransforming))!;
              //         var generic = method.MakeGenericMethod(type);
              //         generic.Invoke(destructure, id => id.ToString());
              //     }
              // 
              //     return config;
              // }
          
              public static void ConfigureGeneratedConverters(this global::Microsoft.EntityFrameworkCore.ModelConfigurationBuilder builder)
              {
                  var types =
                      from global::System.Type type in global::System.Reflection.Assembly.GetAssembly(typeof(global::Generated.StrongIdAttribute))!.GetTypes()
                      let baseType = type.BaseType
                      where baseType?.IsGenericType ?? false
                      where baseType?.GetGenericTypeDefinition() == typeof(global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<,>)
                      let idType = baseType.GetGenericArguments().First()
                      select (idType, type);
          
                  foreach (var (idType, type) in types)
                      builder
                          .Properties(idType)
                          .HaveConversion(type);
              }
          
              public static void ConfigureGeneratedConverters(this global::System.Collections.Generic.IList<global::System.Text.Json.Serialization.JsonConverter> converters)
              {
                  var types =
                      from global::System.Type type in global::System.Reflection.Assembly.GetAssembly(typeof(global::Generated.StrongIdAttribute))!.GetTypes()
                      let baseType = type.BaseType
                      where baseType?.IsGenericType ?? false
                      where baseType?.GetGenericTypeDefinition() == typeof(global::System.Text.Json.Serialization.JsonConverter<>)
                      let idType = baseType.GetGenericArguments().First()
                      select type;
          
                  foreach (var type in types)
                      converters.Add(global::System.Activator.CreateInstance(type) as global::System.Text.Json.Serialization.JsonConverter ?? throw new global::System.InvalidOperationException());
              }
          }
          """;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitializationCallback);

        var syntaxProvider = context.SyntaxProvider
            .CreateSyntaxProvider(SyntacticPredicate, SemanticTransform)
            .Where(static type => type is not null)
            .Select(EntityStrongIdContext.FromEntityTypeInfo!)
            .WithComparer(PartialClassContextEqualityComparer.Instance);

        context.RegisterSourceOutput(syntaxProvider, Execute);
    }

    private static void PostInitializationCallback(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("Generated.Common.g.cs", CommonSrc);
    }

    private static bool SyntacticPredicate(SyntaxNode node, CancellationToken ct) => node switch
    {
        ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclarationSyntax =>
            classDeclarationSyntax.AttributeLists.Any(static attr =>
                attr.Attributes.Any(static a => a.Name.ToString() == "StrongId")),
        RecordDeclarationSyntax { AttributeLists.Count: > 0 } recordDeclarationSyntax =>
            recordDeclarationSyntax.AttributeLists.Any(static attr =>
                attr.Attributes.Any(static a => a.Name.ToString() == "StrongId")),
        _ => false,
    };

    private static INamedTypeSymbol? SemanticTransform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        Debug.Assert(context.Node is ClassDeclarationSyntax or RecordDeclarationSyntax);

        TypeDeclarationSyntax candidate = context.Node switch
        {
            ClassDeclarationSyntax => Unsafe.As<ClassDeclarationSyntax>(context.Node),
            RecordDeclarationSyntax => Unsafe.As<RecordDeclarationSyntax>(context.Node),
            _ => throw new Exception("StrongIdAttribute found on something that is neither a record or a class"),
        };

        if (context.SemanticModel.GetDeclaredSymbol(candidate, ct) is { } entityType)
        {
            var strongIdAttribute = context.SemanticModel.Compilation.GetTypeByMetadataName("Generated.StrongIdAttribute");
            if (strongIdAttribute is not null)
                return entityType;
        }

        return null;
    }

    private static void Execute(SourceProductionContext context, EntityStrongIdContext subject)
    {
        var idClassName = $"{subject.TypeName}Id";

        var idSource = $$"""
                         // <auto-generated/>
                         #pragma warning disable
                         #nullable enable

                         namespace {{subject.Namespace}};

                         {{GeneratedCodeAttribute}}
                         public readonly record struct {{idClassName}}(global::System.Ulid Value) : global::Generated.IStrongId, global::System.IComparable<{{idClassName}}>, global::System.IParsable<{{idClassName}}>
                         {
                             public static {{idClassName}} Empty => new(global::System.Ulid.Empty);
                             public static {{idClassName}} New() => new(global::System.Ulid.NewUlid());
                             public override string ToString() => global::Generated.SourceGeneratorExt.ToSnakeCase(nameof({{idClassName}}).Replace("Id", "")) + $"_{Value.ToString().ToLower()}";
                             public static {{idClassName}} Parse(string s, global::System.IFormatProvider? provider = default) => TryParse(s, provider, out var result) ? result : throw new global::System.FormatException();
                             public static bool TryParse([global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? s, global::System.IFormatProvider? provider, out {{idClassName}} result)
                             {
                                 result = default!;
                         
                                 if (!string.IsNullOrWhiteSpace(s)
                                     && s.Split("_") is [.. var typeNameParts, var idValue]
                                     && string.Join(string.Empty, typeNameParts) is var typeName && typeName == global::Generated.SourceGeneratorExt.ToSnakeCase(nameof({{idClassName}}).Replace("Id", ""))
                                     && global::System.Ulid.TryParse(idValue, out var id))
                                 {
                                     result = new {{idClassName}}(id);
                                     return true;
                                 }
                         
                                 return false;
                             }
                             public int CompareTo({{idClassName}} other) => Value.CompareTo(other);
                         }

                         {{GeneratedCodeAttribute}}
                         public class {{idClassName}}Converter()
                             : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<{{idClassName}}, string>(
                                 id => id.ToString(),
                                 s => {{idClassName}}.Parse(s, null),
                                 new global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints(size: 26));

                         {{GeneratedCodeAttribute}}
                         public sealed class {{idClassName}}ToStringJsonConverter : global::System.Text.Json.Serialization.JsonConverter<{{idClassName}}>
                         {
                             public override {{idClassName}} Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options) => {{idClassName}}.Parse(reader.GetString()!);
                             public override void Write(global::System.Text.Json.Utf8JsonWriter writer, {{idClassName}} value, global::System.Text.Json.JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
                         }
                         """;

        context.AddSource($"Generated.{idClassName}.g.cs", idSource);
    }
}