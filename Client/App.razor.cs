using AzureStaticWebApp.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace AzureStaticWebApp.Client;

public partial class App
{
    [Inject] public LazyAssemblyLoader AssemblyLoader { get; set; } = default!;
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;

    private List<Assembly> LazyLoadedAssemblies { get; set; } = new List<Assembly>();

    private async Task OnNavigateAsync(NavigationContext context)
    {
        if (context.Path.StartsWith("admin"))
        {
            var assemblies = await AssemblyLoader.LoadAssembliesAsync(new[] { "Client.Areas.Admin.dll" });
            LazyLoadedAssemblies.AddRange(assemblies);
        }
    }
}