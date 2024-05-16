using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Presentation.Common;

/// <summary>
/// Strong id model binder for ASP.NET and DDD
/// </summary>
/// <example>
/// [HttpGet("users/{id}")]
/// public void GetUserById([ModelBinder(typeof(StrongIdBinder))] StrongId id) { }
/// </example>
public sealed class StrongIdBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext context)
    {
        var modelName = context.ModelName;
        var modelType = context.ModelType;

        var valueProviderResult = context.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        object? id = TryParse(modelType, valueProviderResult.FirstValue);
        StoreResult(context, modelName, id);

        return Task.CompletedTask;
    }

    private static void StoreResult(ModelBindingContext context, string modelName, object? id)
    {
        if (id is not null)
            context.Result = ModelBindingResult.Success(id);
        else
            context.ModelState.TryAddModelError(modelName, "Invalid Id");
    }

    private static object? TryParse(Type idType, string? rawValue) =>
        FromString(idType, rawValue) is { } parsedValue
            ? Activator.CreateInstance(idType, parsedValue)
            : null;

    private static object? FromString(Type idType, string? rawValue) =>
        rawValue is not null && GetContainedType(idType) is { } containedType
            ? TypeDescriptor.GetConverter(containedType).ConvertFromString(rawValue)
            : null;

    private static Type? GetContainedType(Type idType) =>
        idType.GetProperty("Value")?.PropertyType;
}