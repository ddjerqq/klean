using Blazicons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using TailwindMerge;

namespace Presentation.Components.Ui.Form;

public sealed class FormInputCheckbox : InputCheckbox
{
    public const string BaseClass =
        "peer size-4 shrink-0 rounded-sm border border-primary ring-offset-background focus-visible:outline-none " +
        "focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed " +
        "disabled:opacity-50 data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground";

    [Inject]
    public TwMerge Tw { get; set; } = null!;

    [Parameter]
    public string Class { get; set; } = null!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "button");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "type", "button");
        builder.AddAttribute(3, "aria-checked", CurrentValue);
        builder.AddAttribute(4, "data-state", CurrentValue ? "checked" : "unchecked");

        if (!string.IsNullOrWhiteSpace(NameAttributeValue))
            builder.AddAttribute(5, "name", NameAttributeValue);

        builder.AddAttribute(6, "class", Tw.Merge(BaseClass, CssClass, Class));
        builder.AddAttribute(7, "value", CurrentValue ? "on" : "off");

        builder.AddAttribute(8, "onclick", (Action)(() => CurrentValue = !CurrentValue));

        builder.AddElementReferenceCapture(9, __inputReference => Element = __inputReference);

        builder.AddContent(10, contentBuilder =>
        {
            contentBuilder.OpenElement(1, "span");
            contentBuilder.AddAttribute(2, "data-state", CurrentValue ? "checked" : "unchecked");
            contentBuilder.AddAttribute(3, "class", "flex items-center justify-center text-current");
            contentBuilder.AddAttribute(4, "style", "pointer-events: none;");

            if (CurrentValue)
            {
                contentBuilder.OpenComponent<Blazicon>(5);
                contentBuilder.AddComponentParameter(6, nameof(Blazicon.Svg), Lucide.Check);
                contentBuilder.CloseComponent();
            }

            contentBuilder.CloseElement();
        });

        builder.CloseElement();
    }
}