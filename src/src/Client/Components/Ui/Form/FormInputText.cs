using System.Diagnostics.CodeAnalysis;
using Client.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using TailwindMerge;

namespace Client.Components.Ui.Form;

public sealed class FormInputText : InputBase<string?>
{
    public string BaseClass =
        "peer border-input bg-background ring-offset-background placeholder:text-muted-foreground focus-visible:ring-ring flex h-10 " +
        "w-full rounded-md border px-3 py-2 text-sm file:border-0 file:bg-transparent file:text-sm file:font-medium focus-visible:outline-none " +
        "focus-visible:ring-2 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 " +
        "dark:[&.valid]:!border-green-400 dark:[&.valid]:!ring-green-400 dark:[&.valid]:!text-green-600";

    [Inject]
    public TwMerge Tw { get; set; } = default!;

    [Parameter]
    public string Class { get; set; } = default!;

    [DisallowNull]
    public ElementReference? Element { get; private set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttributeIfNotNullOrEmpty(2, "name", NameAttributeValue);
        builder.AddAttributeIfNotNullOrEmpty(3, "class", Tw.Merge(BaseClass, CssClass, Class));
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