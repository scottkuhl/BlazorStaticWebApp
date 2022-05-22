using AzureStaticWebApp.Common.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace AzureStaticWebApp.Common.Client.Pages.Error;

public partial class Unauthorized
{
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
}