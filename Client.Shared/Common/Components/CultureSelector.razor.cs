using AzureStaticWebApp.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;

namespace AzureStaticWebApp.Client.Shared.Common.Components;

public partial class CultureSelector
{
    private readonly Dictionary<CultureInfo, string> cultures = new()
    {
        { new CultureInfo("en-US"), "English" },
    };

    [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
    [Inject] public NavigationManager Nav { get; set; } = default!;

    private CultureInfo Culture
    {
        get => CultureInfo.CurrentCulture;
        set
        {
            if (CultureInfo.CurrentCulture != value)
            {
                var js = (IJSInProcessRuntime)JSRuntime;
                js.InvokeVoid("blazorCulture.set", value.Name);
                Nav.NavigateTo(Nav.Uri, forceLoad: true);
            }
        }
    }
}