using AzureStaticWebApp.Common.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace AzureStaticWebApp.Common.Client.Components;

public partial class DialogNotification
{
    [Parameter] public Color ButtonColor { get; set; } = default!;
    [Parameter] public string ButtonText { get; set; } = default!;
    [Parameter] public string Content { get; set; } = default!;
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
    [CascadingParameter] public MudDialogInstance MudDialog { get; set; } = default!;

    private void Submit()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }
}