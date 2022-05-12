using AzureStaticWebApp.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace AzureStaticWebApp.Client.Navigation;

public partial class NavMenu
{
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
}
