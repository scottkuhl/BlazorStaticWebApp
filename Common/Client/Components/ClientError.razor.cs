using AzureStaticWebApp.Common.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace AzureStaticWebApp.Common.Client.Components;

public partial class ClientError
{
    [EditorRequired, Parameter] public Exception ErrorContext { get; set; } = default!;
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
}