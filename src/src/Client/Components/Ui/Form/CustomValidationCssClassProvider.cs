using Microsoft.AspNetCore.Components.Forms;

namespace Client.Components.Ui.Form;

public sealed class CustomValidationCssClassProvider(string? validClass, string? inValidClass) : FieldCssClassProvider
{
    public bool ValidateAllFields { get; set; }

    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        var isModified = editContext.IsModified(fieldIdentifier);
        var isValid = !editContext.GetValidationMessages(fieldIdentifier).Any();

        if (isModified || ValidateAllFields)
        {
            return (isValid ? validClass : inValidClass) ?? string.Empty;
        }

        return string.Empty;
    }
}