using AzureStaticWebApp.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace AzureStaticWebApp.Client.Pages;

public partial class LoginProviders
{
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
}
