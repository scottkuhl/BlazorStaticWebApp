using AzureStaticWebApp.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace AzureStaticWebApp.Client.Shared.Common.Components;

public partial class Confirmation
{
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
    [Parameter] public string Content { get; set; } = default!;

    [CascadingParameter] public MudDialogInstance MudDialog { get; set; } = default!;

    private void No()
    {
        MudDialog.Close(DialogResult.Cancel());
    }

    private void Yes()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }
}