using AzureStaticWebApp.Common.Shared.Resources;
using Majorsoft.Blazor.Components.GdprConsent;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace AzureStaticWebApp.Common.Client.Components;

public partial class GdprConsents
{
    private GdprBanner _gdprBanner = default!;
    private List<GdprConsentDetail> _gdprConsents = default!;
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
    [Parameter] public MudTheme Theme { get; set; } = new MudTheme();

    protected override void OnInitialized()
    {
        _gdprConsents = new List<GdprConsentDetail>()
        {
            new GdprConsentDetail() { ConsentName = "Required", IsAccepted = true },
            new GdprConsentDetail() { ConsentName = "Session", IsAccepted = true },
            new GdprConsentDetail() { ConsentName = "Tracking", IsAccepted = true },
        };
    }
}