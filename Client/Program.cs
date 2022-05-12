using AzureStaticWebApp.Client;
using AzureStaticWebApp.Client.Shared.Common.Extensions;
using AzureStaticWebApp.Client.Shared.Common.Services;
using AzureStaticWebApp.Client.Shared.Services;
using Majorsoft.Blazor.Components.GdprConsent;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Azure.Functions.Authentication.WebAssembly;
using MudBlazor;
using MudBlazor.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("AzureStaticWebApp", (sp, client) =>
{
    client.BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress);
    _ = client.EnableIntercept(sp);
});
builder.Services.AddScoped(x => x.GetService<IHttpClientFactory>().CreateClient("AzureStaticWebApp"));
builder.Services.AddHttpClientInterceptor();
builder.Services.AddScoped<HttpInterceptorService>();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.MaxDisplayedSnackbars = 1;
});

builder.Services.AddStaticWebAppsAuthentication();
builder.Services.AddLoadingBar();
builder.Services.AddGdprConsent();
builder.Services.AddLocalization();

builder.Services.AddScoped<IMovieHttpService, MovieHttpService>();

builder.UseLoadingBar();

var host = builder.Build();
await host.SetDefaultCulture();
await host.RunAsync();