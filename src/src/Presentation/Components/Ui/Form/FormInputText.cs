using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Presentation.Common;
using TailwindMerge;

namespace Presentation.Components.Ui.Form;

public sealed class FormInputText : InputBase<string?>
{
    public const string BaseClass =
        "peer flex h-9 w-full rounded-md border border-input bg-background px-3 py-1 text-sm shadow-sm transition-colors " +
        "file:border-0 file:bg-background file:text-sm file:font-medium file:text-foreground placeholder:text-muted-foreground " +
        "focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50 md:text-sm " +
        "[&.valid]:!border-green-700 [&.valid]:!ring-green-700 [&.valid]:!text-green-700 " +
        "[&.invalid]:!border-destructive [&.invalid]:!ring-destructive [&.invalid]:!text-destructive ";

    [Inject]
    public TwMerge Tw { get; set; } = null!;

    [Parameter]
    public string Class { get; set; } = null!;

    [DisallowNull]
    public ElementReference? Element { get; private set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttributeIfNotNullOrWhiteSpace(2, "name", NameAttributeValue);
        builder.AddAttributeIfNotNullOrWhiteSpace(3, "class", Tw.Merge(Class, BaseClass, CssClass));
        builder.AddAttribute(4, "value", CurrentValueAsString);
        builder.AddAttribute(5, "onchange", EventCallback.Factory.CreateBinder<string?>(this, value => CurrentValueAsString = value, CurrentValueAsString));
        builder.SetUpdatesAttributeName("value");
        builder.AddElementReferenceCapture(6, inputReference => Element = inputReference);
        builder.CloseElement();
    }

    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out string? result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        result = value;
        validationErrorMessage = null;
        return true;
    }
}