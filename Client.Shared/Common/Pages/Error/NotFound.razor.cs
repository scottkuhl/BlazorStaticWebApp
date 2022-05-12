using AzureStaticWebApp.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace AzureStaticWebApp.Client.Shared.Common.Pages.Error;
public partial class NotFound
{
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
}
