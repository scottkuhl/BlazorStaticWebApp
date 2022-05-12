using AzureStaticWebApp.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace AzureStaticWebApp.Client.Shared.Common.Components;

public partial class ClientError
{
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
    [EditorRequired, Parameter] public Exception ErrorContext { get; set; } = default!;
}