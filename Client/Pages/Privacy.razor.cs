using AzureStaticWebApp.Common.Client.Components;
using AzureStaticWebApp.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace AzureStaticWebApp.Client.Pages;

public partial class Privacy
{
    [Inject] public IDialogService Dialog { get; set; } = default!;
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
    [Inject] public NavigationManager Nav { get; set; } = default!;

    private async Task DeleteAsync()
    {
        var parameters = new DialogParameters
        {
            { "Content", $"Are you sure you want to delete all your data?  This can not be undone." }
        };

        var dialog = Dialog.Show<Confirmation>("Delete Your Data", parameters, new DialogOptions { DisableBackdropClick = true });

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            Nav.NavigateTo("/.auth/purge/aad");
        }
    }
}